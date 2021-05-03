using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XAMCommon.Trace;
using XAMIO.Base;
using XAMIO.Common.Net;
using XAMIO.Ulux;
using XAMIO.Ulux.Ump;
using XAMIO.Ulux.Ump.Message;
using XAMIO.Ulux.Ump.Telegram;

namespace XAMIO.UmpClient
{
    /// <summary>
    /// 
    /// </summary>    
    class XAMUmpClientCom : XAMIOConnectionBase<XAMUmpClientItem>
    {
        #region data_and_constructor

        XAMUmpClientConfig Config;
        XAMUmpClientItem item;

        XAMUmpStream client;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">process info</param>
        public XAMUmpClientCom(XAMCommon.ServiceReference.XAMIOProcessInfo p, XAMUmpClientConfig config, string conId, XAMUmpDispatcher dispatcher)
            : base(null, p, conId, config.XAMServerIP,IOStandbyMode.Mode.Read)
        {
            string[] split = conId.Split(':');
            string destination = split[0];
            int projectID = System.Convert.ToInt16(split[1]);
            int firmwareVersion=System.Convert.ToInt16(split[2]);
            int switchID = System.Convert.ToInt16(split[3]);
            int designID = System.Convert.ToInt16(split[4]); 

            this.Config = config;

            client = new XAMUmpStream(dispatcher,destination, projectID, firmwareVersion, switchID, designID,Trace);
            client.ValueEvent += client_ValueEvent;
        }
             

        #endregion

        #region connection_to_xamserver
        public override void EnableClient()
        {
            try
            {
            }
            catch (Exception ex)
            {
                base.Trace("EnableServer(): " + ex.Message, TracePrio.ERROR);
            }
        }

        public override void DisableClient()
        {
            try
            {
            }
            catch (Exception ex)
            {
                base.Trace("DisableClient(): " + ex.Message, TracePrio.ERROR);

            }
        }
        public override void DisposeConnection()
        {
            Dispose();
        }


        protected override XAMIOConnectionStatus CheckAndConnect()
        {  
            XAMIOConnectionStatus status = client.CheckAndConnect();
            if(status != XAMIOConnectionStatus.Connected)
            {
                foreach (KeyValuePair<string, XAMIOValue> element in VariableList)
                 {
                     item = element.Value as XAMUmpClientItem;
                     item.IsInitialized = false;
                 }
            }
            return status;
        }
        
        #endregion

        #region cyclic_data_handling

        public override void StopCommunication()
        {
            try
            {
                if (client != null && client.IsConnected)
                    client.Stop();
            }
            catch (Exception ex)
            {
                base.Trace("StopCommunication() exception: " + ex.Message, TracePrio.ERROR);
            }
        }

        public override void DoRWDisabled()
        {
            try
            {
                if (client != null && client.IsConnected)
                    client.Stop();
            }
            catch (Exception ex)
            {
                base.Trace("DoRWDisabled() exception: " + ex.Message, TracePrio.ERROR);
            }
        }

        
        void client_ValueEvent(UmpMessageID msgID, int actorID, byte[] value)
        {
            string Ident = XAMUmpClientItem.CreateIdentifier(actorID, msgID, false);
            var item = VariableList.GetItem(Ident) as XAMUmpClientItem;
            if (item == null)
            {
                Trace("receive message actor <" + actorID + "> message ID <" + msgID + "> not found ", TracePrio.WARNING);
                return;
            }

            if (!item.IsReadAccess)
            {
                Trace("receive message  <" + actorID + "> message ID <" + msgID + "> is write only ", TracePrio.WARNING);
                return;
            }

            switch (msgID)
            {
                
                case UmpMessageID.EditValue:
                    int editValue = BitConverter.ToInt16(value, 0);                   
                    base.Trace("ValueEvent '" + Ident + "' received edit value <" + editValue + ">", TracePrio.MESSAGE);
                    item.SetValidReaded(editValue, "ok", DateTime.Now);
                    item.IsInitialized = true;
                    break;
                case UmpMessageID.I2C_Temperature:
                    int temp = BitConverter.ToInt16(value, 0);
                    byte valid = value[2];
                    if (valid > 0)
                    {
                        Trace("ValueEvent '" + Ident + "' received temperature <" + temp + ">", TracePrio.MESSAGE);
                        item.SetValidReaded(temp, "ok", DateTime.Now);
                        item.IsInitialized = true;
                    }
                    else
                        item.SetInvalidReaded(XAMCommon.PropertyQuality.Bad, "not valid <" + valid + ">");
                break;
            }
        }   

        int CycleReadCnt = 0;
        public override int DoRead()
        {
            try
            {        
                //else if(doSend2)
                //{
                //    short red_mask = 0x7C00;
                //    short green_mask =  0x3E0;
                //    short blue_mask = 0x1F;


                //    for (int j = 0; j < 90; j += 5)
                //    {
                //        List<byte> videoData = new List<byte>();

                //        for (int i = 0; i < 86 * 5; i++)
                //        {
                //            videoData.Add((byte)((blue_mask) & 0xFF));
                //            videoData.Add((byte)((blue_mask >> 8) & 0xFF));


                //        }

                //        XAMUmpVideoStreamTelegram v = new XAMUmpVideoStreamTelegram(SequenceID, (short)(j), 5, videoData.ToArray(), Trace);
                //        sendTel.VideoStream = v;
                //        dispatcher.Send(sendTel, destination);
                //        System.Threading.Thread.Sleep(5);
                //    } 
                    
                //    //doSend2 = false;

                //}


                foreach (KeyValuePair<string, XAMIOValue> element in VariableList)
                {
                    item = element.Value as XAMUmpClientItem;

                    if (!item.IsReadAccess)
                        continue;
                    
                    switch (item.MessageId)
                    {
                        case UmpMessageID.EditValue:
                            if (!item.IsInitialized)
                            {
                                // request value;
                                client.Send(new XAMUmpMessageTelegram(UmpMessageID.EditValue, item.ActorId, new byte[0], Trace));  
                                base.Trace("DoRead '" + element.Key + "' request EditValue actorID <" + item.ActorId + ">", TracePrio.MESSAGE);
                            }
                            break;
                        case UmpMessageID.I2C_Temperature:
                            // request value;
                            client.Send(new XAMUmpMessageTelegram(UmpMessageID.I2C_Temperature, item.ActorId, new byte[0], Trace));
                            base.Trace("DoRead '" + element.Key + "' request I2C_Temperature actorID <" + item.ActorId + ">", TracePrio.MESSAGE);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                base.Trace("Error in DoRead(): " + e.Message, TracePrio.ERROR);
            }
            

            int tmp = CycleReadCnt;
            CycleReadCnt = 0;
            return tmp;
        }
        int CycleWriteCnt = 0;
        public override int DoWrite()
        {
            foreach (KeyValuePair<string, XAMIOValue> element in VariableList)
            {
                item = element.Value as XAMUmpClientItem;

                if (!item.IsWriteAccess)
                    continue;

                try
                {
                    bool hasChanged = item.IsChangedWrite;

                    //// valid handling of value
                    ////bool isValueOnline = item.IsValueOnline;
                    bool isControllerOnline = item.IsControllerOnline;
                    bool isValid = item.IsValid;
                    object Value = item.Value;

                    if (Value == null)
                        continue;

                    
                    if (!hasChanged)
                        continue;

                    List<XAMUmpMessageTelegram> msgs = new List<XAMUmpMessageTelegram>();

                    switch( item.MessageId)
                    {
                        case UmpMessageID.EditValue:
                            byte[] sndVal = BitConverter.GetBytes(System.Convert.ToInt16(Value));                    
                            msgs.Add( new XAMUmpMessageTelegram(UmpMessageID.EditValue, item.ActorId, sndVal, Trace));
                            base.Trace("DoWrite '" + element.Key + "' write edit value <" + Value +">", TracePrio.MESSAGE);
                            break;
                        case UmpMessageID.DateTime:
                            List<byte> sndDatetime= new List<byte>();
                            sndDatetime.Add((byte)DateTime.Now.Second);
                            sndDatetime.Add((byte)DateTime.Now.Minute);
                            sndDatetime.Add((byte)DateTime.Now.Hour);
                            sndDatetime.Add((byte)DateTime.Now.DayOfWeek);
                            sndDatetime.Add((byte)DateTime.Now.Day);
                            sndDatetime.Add((byte)(DateTime.Now.Year & 0xFF));
                            sndDatetime.Add((byte)((DateTime.Now.Year >> 8) & 0xFF));
                             msgs.Add( new XAMUmpMessageTelegram(UmpMessageID.DateTime, item.ActorId, sndDatetime.ToArray(), Trace));
                            base.Trace("DoWrite '" + element.Key + "' write time value <" + DateTime.Now.ToString() + ">", TracePrio.MESSAGE);
                            break;
                    }

                    if ( msgs.Count == 0)
                        continue;
                    client.Send(msgs);
                    CycleWriteCnt++;
                    item.SetValidWritten("ok", Value, DateTime.Now, isControllerOnline, isValid);
                }
                catch (Exception ex)
                {
                    item.SetInvalidWritten(XAMCommon.PropertyQuality.Bad,"do write exception: " + ex.Message);
                    base.Trace("DoWrite '" + element.Key + "' exception: " + ex.Message, TracePrio.ERROR);
                }
            }            
            return CycleWriteCnt;
        }
        #endregion

        #region cfg and status functions
        protected override void ConfigChanged(List<string> parameters)
        {
            int cfgPntr = 0;
            foreach (string cfgParm in base.CfgParameters)
            {
                switch (cfgPntr)
                {
                    case 0:
                        try
                        {
                            //password = cfgParm;
                        }
                        catch
                        {
                            Trace("Error get password from config - default value will be used", TracePrio.ERROR);
                        }
                        break;
                    case 1:
                        try
                        {
                            //SendAllDataOnConnect = System.Convert.ToBoolean(cfgParm);
                        }
                        catch
                        {
                            Trace("Error get SendAllDataOnConnect config", TracePrio.ERROR);
                        }
                        break;
                    case 2:
                        try
                        {
                            //LoginNeeded = System.Convert.ToBoolean(cfgParm);
                        }
                        catch
                        {
                            Trace("Error get login needed config", TracePrio.ERROR);
                        }
                        break;
                }
                cfgPntr++;
            }    
            }
        protected override List<string> SetStatus()
        {
            if (client == null)
                return new List<string> { "ump client not initialized"};

            if (!client.IsConnected)
                return new List<string> { "ump client not connected" };

            if (!client.Initialized)
                return new List<string> { "ump client not initialized" };

            List<string> SessionsStatus = new List<string>();
            SessionsStatus.Add("Connected");            
            return SessionsStatus;
        }
        #endregion

        #region create properties
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="bitindex"></param>
        /// <param name="complexpath"></param>
        /// <returns></returns>
        public override XAMUmpClientItem UpdateValue(XAMIOTrace trace, XAMUmpClientItem item, XAMDriverPropertiesExtended config, out int? bitindex, out string complexpath)
        {
            return XAMUmpClientItem.ParseConfig(trace, item, config, out bitindex, out complexpath);
        }
        #endregion

    }


}

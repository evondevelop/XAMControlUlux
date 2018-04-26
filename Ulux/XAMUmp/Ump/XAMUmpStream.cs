using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XAMCommon.Trace;
using XAMIO.Base;
using XAMIO.Common.Net;
using XAMIO.Ulux.Ump;
using XAMIO.Ulux.Ump.Message;
using XAMIO.Ulux.Ump.Telegram;

namespace XAMIO.Ulux
{
    public class XAMUmpStream
    {
        #region private properties
        private XAMUmpDispatcher dispatcher;

         IPAddress destination;
         string remoteAddressIdent;

         int packageID = 0;
         int projectID;
         int firmwareVersion;
         int switchID;
         int designID;

         XAMUmpTelegram umpTelegram;
         TraceDelegate trace;

         public bool Initialized
         {
             get
             {
                 if (!IdState.NotInitialized && !IdState.InitRequest) 
                     return true;
                 return false;
             }
         }
         int waitPacketID = -1;
        #endregion

         #region public properties

         XAMUmpStateFlags _IdState = new XAMUmpStateFlags();
         public XAMUmpStateFlags IdState
         {
             get { return _IdState; }
         }
         int _PageCount = -1;
         public int PageCount
         {
             get { return _PageCount; }
         }
         int _IdControl = -1;
         public int IdControl
         {
             get { return _IdControl; }
         }
         int _PageIndex = -1;
         public int PageIndex
         {
             get { return _PageIndex; }
         }

         public delegate void AcknowledgeTelegram();
         public event AcknowledgeTelegram AckReceived;

         #endregion

         #region constructor
         public XAMUmpStream(XAMUmpDispatcher dispatcher, string ipAdr, int projectID, int firmwareVersion, int switchID, int designID, TraceDelegate trace)
         {
             this.dispatcher = dispatcher;
             this.trace = trace;

             this.projectID = projectID;
             this.firmwareVersion = firmwareVersion;
             this.switchID = switchID;
             this.designID = designID;

             destination = IPAddress.Parse(ipAdr);

             remoteAddressIdent = XAMUmUtils.GetSwitchIdentifier(projectID, switchID, designID);
             umpTelegram = new XAMUmpTelegram(packageID, projectID, firmwareVersion, switchID, designID, trace);
         }

        #endregion

         #region connection handling
         /// <summary>
         /// Checks the and connect.
         /// </summary>
         /// <returns></returns>
         public XAMIOConnectionStatus CheckAndConnect()
         {
             if (dispatcher.CheckAndConnect() != XAMIOConnectionStatus.Connected)
             {
                 _IdState = new XAMUmpStateFlags();
                 return XAMIOConnectionStatus.notConnected;
             }
             HandleTelegramReceived();
             if( IdState.InitRequest)
             {
                 // Send Init Request
                 SetIDControl();
                 return XAMIOConnectionStatus.notConnected;
             }

             if( IdState.TimeRequest )
                 Send(XAMUmpDateTime.Create(DateTime.Now,trace));

             if (DateTime.Now.Subtract(IdState.LastStateReceived).Seconds > 10)
                 SendIDStateIDControlRequest();
                      
             return XAMIOConnectionStatus.Connected;
         }

         /// <summary>
         /// Gets a value indicating whether this instance is connected.
         /// </summary>
         /// <value>
         /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
         /// </value>
         public bool IsConnected
         {
             get
             {
                 if (dispatcher == null)
                     return false;
                 return dispatcher.IsConnected;
             }
         }

         /// <summary>
         /// Stops this instance.
         /// </summary>
         public void Stop()
         {
             //dispatcher.Stop();
         }
        #endregion

         #region send

         public void Send(List<XAMUmpMessageTelegram> msgs)
         {
             umpTelegram.PackageID = packageID;
             umpTelegram.Messages = msgs;
             dispatcher.Send(umpTelegram, destination);
             packageID++;
         }
         public void Send(XAMUmpMessageTelegram msg)
         {
             umpTelegram.VideoStream = null;
             Send(new List<XAMUmpMessageTelegram>() { msg });
         }
         public void Send(XAMUmpVideoStreamTelegram msg)
         {
             umpTelegram.PackageID = packageID;
             umpTelegram.Messages = null;
             umpTelegram.VideoStream = msg;
             dispatcher.Send(umpTelegram, destination);
             packageID++;
         }
         public void Send(XAMUmpMessageTelegram msg1, XAMUmpMessageTelegram msg2)
         {
             umpTelegram.VideoStream = null;
             Send(new List<XAMUmpMessageTelegram>() { msg1, msg2 });
         }

         #endregion


         #region telegrams
         public bool ConnectionCheck()
         {
             try
             {
                 //return true;
                 trace("Connection check  request init for control and state!", TracePrio.MESSAGE);

                 HandleTelegramReceived();        // clear receive buffer upfront    

                 IDStateIDControlRequestBlocking();

                 SetIDControl();

                 trace("Connection check done: state <" + IdState + "> control <" + IdControl + ">", TracePrio.MESSAGE);
                 return true;
             }
             catch (Exception ex)
             {
                 trace("Init failed ex: " + ex.Message, TracePrio.NOTIFICATION);
             }
             return false;
         }

         

        public void SendIDStateIDControlRequest()
         {
             XAMUmpMessageTelegram msg1 = new XAMUmpMessageTelegram(UmpMessageID.IdState, 0, new byte[0], trace);
             XAMUmpMessageTelegram msg2 = new XAMUmpMessageTelegram(UmpMessageID.IdControl, 0, new byte[0], trace);
             Send(msg1, msg2);
         }

        public void SendPageCountPageIndexRequest()
        {
            XAMUmpMessageTelegram msg1 = new XAMUmpMessageTelegram(UmpMessageID.PageCount, 0, new byte[0], trace);
            XAMUmpMessageTelegram msg2 = new XAMUmpMessageTelegram(UmpMessageID.PageIndex, 0, new byte[0], trace);
            Send(msg1, msg2);
        }

        private void SetIDControl()
        {
            Send(new XAMUmpMessageTelegram(UmpMessageID.IdControl, 0, new byte[4], trace));
        }

        public void IDStateIDControlRequestBlocking()
        {
            trace("Init request init for control and state!", TracePrio.MESSAGE);
            _IdControl = -1;
            _IdState = new XAMUmpStateFlags();
            waitPacketID = packageID;

            SendIDStateIDControlRequest();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (_IdControl == -1 || IdState.NotInitialized)
            {
                HandleTelegramReceived();
                if (sw.ElapsedMilliseconds > 1000)
                    throw new Exception("init 1 timeout!");
            }
            sw.Stop();
        }

        public void PageCountPageIndexRequestBlocking()
        {
            trace("Init request init for page count and page index!", TracePrio.MESSAGE);
            _PageCount = -1;
            _PageIndex = -1;
            waitPacketID = packageID;

            SendPageCountPageIndexRequest();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (_PageCount == -1 || _PageIndex == -1)
            {
                HandleTelegramReceived();
                if (sw.ElapsedMilliseconds > 1000)
                    throw new Exception("init 1 timeout!");
            }
            sw.Stop();
        }


        public bool InitRequest()
        {
            try
            {
                packageID = 1;

                IDStateIDControlRequestBlocking();
                PageCountPageIndexRequestBlocking();

                SetIDControl();

                System.Threading.Thread.Sleep(500);

                trace("Init send datetime!", TracePrio.MESSAGE);                
                waitPacketID = packageID;
                Send(XAMUmpDateTime.Create(DateTime.Now, trace));

                trace("Init done: state <" + IdState + "> control <" + IdControl + "> PageCount <" + PageCount + "> PageIndex <" + PageIndex + ">", TracePrio.NOTIFICATION);
                return true;
            }
            catch (Exception ex)
            {
                trace("Init failed ex: " + ex.Message, TracePrio.NOTIFICATION);
            }
            return false;
        }

        #endregion

        #region receive

        public delegate void ReceivedValue(UmpMessageID msgID, int actorID, byte[] value);
        public delegate void ReceivedVideoState(int StateFlags, int BoundsLeft, int BoundsTop,int BoundsRight,int BoundsBottom);
        public event ReceivedValue ValueEvent;
        public event ReceivedVideoState VideoStateEvent;

        /// <summary>
        /// Handles the telegram received.
        /// </summary>
        public void HandleTelegramReceived()
        {
            try
            {
                TelegramReceivedEventArgs<XAMUmpTelegram> e = null;
                do
                {
                    e = dispatcher.GetNextRcvTel(remoteAddressIdent);
                    if (e == null)
                        break;
                                       
                    // If there is no message in the telegrame --> it is an Ack!
                    if (AckReceived != null /*&& e.Telegram.Messages.Count == 0*/)
                        AckReceived();

                    foreach (var msg in e.Telegram.Messages)
                    {
                        try
                        {
                            switch (msg.MessageID)
                            {
                                case UmpMessageID.IdState:
                                    _IdState =new XAMUmpStateFlags(msg.Value);
                                    break;
                                case UmpMessageID.PageCount:
                                    _PageCount = BitConverter.ToInt16(msg.Value, 0);
                                    break;
                                case UmpMessageID.IdControl:
                                    _IdControl = BitConverter.ToInt16(msg.Value, 0);
                                    break;
                                case UmpMessageID.PageIndex:
                                    _PageIndex = BitConverter.ToInt16(msg.Value, 0);
                                    break;

                                case UmpMessageID.EditValue:
                                case UmpMessageID.I2C_Temperature:
                                    if (ValueEvent == null)
                                        break;
                                    ValueEvent(msg.MessageID, msg.ActorID, msg.Value);
                                    break;
                                case UmpMessageID.VideoState:
                                    if (msg.Value.Length < 12)
                                    {
                                        trace("rcv VideoState: to less data", TracePrio.ERROR);
                                        break;
                                    }
                                    int cnt = 0;
                                    int StateFlags = BitConverter.ToInt32(msg.Value, cnt); cnt += 4;
                                    int BoundsLeft = BitConverter.ToInt16(msg.Value, cnt); cnt += 2;
                                    int BoundsTop = BitConverter.ToInt16(msg.Value, cnt); cnt += 2;
                                    int BoundsRight = BitConverter.ToInt16(msg.Value, cnt); cnt += 2;
                                    int BoundsBottom = BitConverter.ToInt16(msg.Value, cnt); cnt += 2;
                                    trace("HandleTelegramReceived video state '" + StateFlags + "' left '" + BoundsLeft + "' top '" + BoundsTop + "' right '" + BoundsRight + "' bottom '" + BoundsBottom + "' ", TracePrio.MESSAGE);
                                    if (VideoStateEvent !=null)
                                        VideoStateEvent(StateFlags, BoundsLeft, BoundsTop, BoundsRight, BoundsBottom);

                                    break;
                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            trace("receive message exception: " + ex.Message, TracePrio.ERROR);
                        }
                    }
                } while (e != null);

            }
            catch (Exception ex1)
            {
                trace("receive exception: " + ex1.Message, TracePrio.ERROR);
            }

        }
        #endregion

    }
}

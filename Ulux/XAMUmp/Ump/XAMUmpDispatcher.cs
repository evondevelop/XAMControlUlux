using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XAMIO.Common.Net;
using XAMCommon.Trace;
using XAMIO.Common;
using XAMIO.Ulux.Ump.Telegram;
using XAMIO.Base;

namespace XAMIO.Ulux.Ump
{
    public class XAMUmpDispatcher
    {
        private UdpListenerAsync<XAMUmpTelegram> client;

        private TraceDelegate Trace;

        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpTelegram"/> class.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public XAMUmpDispatcher(TraceDelegate trace)
        {
            this.Trace = trace;
        }

        #region connecting

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get {
                if (client == null)
                    return false;
                return client.IsConnected;
            }
        }
        /// <summary>
        /// Checks the and connect.
        /// </summary>
        /// <returns></returns>
        public XAMIOConnectionStatus CheckAndConnect()
        {
            if (client == null)
            {
                client = new UdpListenerAsync<XAMUmpTelegram>(0x88AC, Trace);
                client.TelegramReceived += client_TelegramReceived;                
            }

            if (!client.IsConnected)
            {
                client.Connect();
                //System.Threading.Thread.Sleep(200);
                client.Start();
            }

            if (!client.IsConnected)
            {
                return XAMIOConnectionStatus.notConnected;
            }
            return XAMIOConnectionStatus.Connected;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            client.Disconnect();
        }
        #endregion

        #region send
        private object sendlock = new object();
        /// <summary>
        /// Sends the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="destination">The destination.</param>
        public void Send( XAMUmpTelegram request, System.Net.IPAddress destination)
        {
            lock (sendlock)
            {
                client.Send(request, destination);
            }
        }

        #endregion

        #region receive
        /// <summary>
        /// The receivebuffer
        /// </summary>
        Dictionary<string, List<TelegramReceivedEventArgs<XAMUmpTelegram>>> receivebuffer = new Dictionary<string, List<TelegramReceivedEventArgs<XAMUmpTelegram>>>();

        /// <summary>
        /// Handles the TelegramReceived event of the client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TelegramReceivedEventArgs{XAMUmpTelegram}"/> instance containing the event data.</param>
        void client_TelegramReceived(object sender, TelegramReceivedEventArgs<XAMUmpTelegram> e)
        {
            
            try
            {
                string remoteaddr = XAMUmUtils.GetSwitchIdentifier(e.Telegram.ProjectID, e.Telegram.SwitchId, e.Telegram.DesignId);
                 List<TelegramReceivedEventArgs<XAMUmpTelegram>> devBuf;
                 if (!receivebuffer.TryGetValue(remoteaddr, out devBuf))
                 {
                     devBuf = new List<TelegramReceivedEventArgs<XAMUmpTelegram>>();
                     receivebuffer.Add(remoteaddr, devBuf);
                 }

                if (devBuf == null)
                    devBuf = new List<TelegramReceivedEventArgs<XAMUmpTelegram>>();

                lock (receivebuffer)
                {
                    if (devBuf.Count > 100)
                    {
                        Trace("To many telegrams in receive buffer of <" + remoteaddr + "> - cleare it!", TracePrio.FATALERROR);
                        devBuf.Clear();
                    }

                    devBuf.Add(e);
                }

            }
            catch (Exception ex1)
            {
                Trace("receive exception: " + ex1.Message, TracePrio.ERROR);
            }
        }

        /// <summary>
        /// Gets the next RCV tel.
        /// </summary>
        /// <param name="remoteaddr">The remoteaddr.</param>
        /// <returns></returns>
        public TelegramReceivedEventArgs<XAMUmpTelegram> GetNextRcvTel(string remoteaddr)
        {
            List<TelegramReceivedEventArgs<XAMUmpTelegram>> devBuf;
            if (!receivebuffer.TryGetValue(remoteaddr, out devBuf))
            {
                return null;
            }
             if (devBuf.Count <= 0)
                 return null;

             lock (receivebuffer)
             {
                 var tel = devBuf[0];
                 devBuf.RemoveAt(0);
                 return tel;
             }
        }

        #endregion
    }
}

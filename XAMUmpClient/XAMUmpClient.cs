using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XAMIO.Base;

namespace XAMIO.UmpClient
{
    /// <summary>
    /// XAM IO class for OPC UA Client Driver interconnection
    /// </summary>
    class XAMUmpClient : XAMIOBase
    {
        XAMUmpClientConfig Config;

        static XAMIO.Ulux.Ump.XAMUmpDispatcher dispatcher;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StartImmediate">start working thread immediate</param>
        /// <param name="Name">Driver Name</param>
        /// <param name="XAMServerIP">XAMServer IP</param>
        public XAMUmpClient(bool StartImmediate, string XAMServerIP, XAMUmpClientConfig config)
            : base(null, XAMUmpClientService.XAMUmpClientName, XAMServerIP, StartImmediate, true, XAMVarListMode.IOVariablesFromXAMServer)
        {
            Config = config;

            try
            {
                dispatcher = new XAMIO.Ulux.Ump.XAMUmpDispatcher(base.Trace);
            }
            catch
            {
               
            }
        }

        /// <summary>
        /// create a new IO connection
        /// </summary>
        /// <param name="IOName">Driver name</param>
        /// <param name="ConnectionID">the connection id --> must be localhost</param>
        /// <returns>a new driver connection</returns>
        public override XAMConnectionCollection<IIOConnection> CreateNewIOConnection(List<string> ConnectionIDs)
        {       
            if (ConnectionIDs.Count != 1)
                throw new Exception("only one connection Id allowed");

            foreach (string conId in ConnectionIDs)
            {
                XAMUmpClientCom client = new XAMUmpClientCom(base.ProcessInfo, Config, conId, dispatcher);
                XAMConnectionCollection<IIOConnection> list = new XAMConnectionCollection<IIOConnection>();
                list.Add(client);
                return list;
            }

            throw new Exception("no connection");
        }

        /// <summary>
        /// override this function with your specific driver config check
        /// throw an exception if the config is not valid
        /// </summary>
        /// <param name="IOName">IO name</param>
        /// <param name="ConnectionID">the connection id</param>
        public override void CheckIOConfig(List<string> ConnectionIDs)
        {           
            if (ConnectionIDs.Count != 1)
                throw new Exception("only one connection ID is allowed");
            foreach (var ConnectionID in ConnectionIDs)
            {
                string[] split = ConnectionID.Split(':');
                string IpAddress = split[0];
                int projectID = System.Convert.ToInt16(split[1]);
                int firmwareVersion = System.Convert.ToInt16(split[2]);
                int switchID = System.Convert.ToInt16(split[3]);
                int designID = System.Convert.ToInt16(split[4]);

                if (!XAMIO.Common.IPAddressExtensions.IsValidIP(IpAddress))
                    throw new Exception("Invalid connectionID <" + ConnectionID + ">- it has to be a valid ip address");
            }
        }

    }

     
}

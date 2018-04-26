using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XAMIO.UmpClient
{
    

    [XmlRoot("XAMUmpClient")]
    public class XAMUmpClientConfig : XAMCommon.Services.XAMConfigBase
    {
       
    }

    [System.ComponentModel.DesignerCategory("")]
    public class XAMUmpClientService : XAMCommon.Services.XAMWinServiceBase<XAMUmpClientConfig>
    {
        public const string XAMUmpClientName = "XAMUmpClient";
        protected override XAMCommon.Services.IXAMService CreateService(string xamserverIP, bool startImmediate, XAMUmpClientConfig config)
        {
            return new XAMUmpClient(startImmediate, xamserverIP, config);
        }

         static void Main(string[] Args)
        {
            XAMUmpClientService opcUaService = new XAMUmpClientService(Args);
        }

        public XAMUmpClientService()
             : base(XAMUmpClientName)
        {
        }

        public XAMUmpClientService(string[] Args)
            : base(XAMUmpClientName, Args)
        {
        }
    }

    [RunInstaller(true)]
    [System.ComponentModel.DesignerCategory("")]
    public class XAMUmpClientServiceInstaller : XAMCommon.Services.ServiceInstallerBase
    {
        public XAMUmpClientServiceInstaller()
            : base(XAMUmpClientService.XAMUmpClientName, "XAM u::Lux Message Protocol (UMP) Client")
        {
        }

    }
}

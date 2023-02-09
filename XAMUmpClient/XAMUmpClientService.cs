using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using XAMCommon.Services;
using XAMIO.Base.Service;

namespace XAMIO.UmpClient
{
    

    [XmlRoot("XAMUmpClient")]
    public class XAMUmpClientConfig : XAMCommon.Services.XAMConfigBase
    {
       
    }

    [System.ComponentModel.DesignerCategory("")]
    public class XAMUmpClientService : XAMIOBaseService<XAMUmpClientConfig>
    {
        public const string XAMUmpClientName = "XAMUmpClient";
        protected override IXAMService CreateService(string xamserverIP, XAMUmpClientConfig config, bool startImmediate)
        {
            return new XAMUmpClient(startImmediate, xamserverIP, config);
        }

        static void Main(string[] Args)
        {
            _ = new XAMUmpClientService(Args);
        }

        public XAMUmpClientService()
             : base(null, XAMUmpClientName)
        {
        }

        public XAMUmpClientService(string[] Args)
            : base(null, XAMUmpClientName, Args)
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

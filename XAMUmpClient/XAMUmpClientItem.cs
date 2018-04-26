using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XAMCommon.Trace;
using XAMIO.Base;
using XAMIO.Ulux.Ump;

namespace XAMIO.UmpClient
{
    /// <summary>
    /// commandline fusion Item
    /// </summary>
    class XAMUmpClientItem : XAMIOValue
    {
        /// <summary>
        /// MessageID
        /// </summary>
        public UmpMessageID MessageId;

        /// <summary>
        /// Actor ID
        /// </summary>
        public int ActorId;

        /// <summary>
        /// The is initialized
        /// </summary>
        public bool IsInitialized = false;
        
        // <summary>
        ///commandline fusion Item
        /// </summary>
        public XAMUmpClientItem()
            : base()
        {
            
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public override string Identifier
        {
            get { return CreateIdentifier(ActorId, MessageId, base.IsWriteAccess?true:false); }
        }

        public override void CheckConfiguration(IXAMValue xamValue)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActorId"></param>
        /// <param name="MessageId"></param>
        /// <param name="Writeable"></param>
        /// <returns></returns>
        public static string CreateIdentifier(int ActorId, UmpMessageID MessageId, bool Writeable)
        {
            return ActorId.ToString() + MessageId.ToString() + Writeable.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="bitindex"></param>
        /// <param name="complexPath"></param>
        /// <returns></returns>
        public static XAMUmpClientItem ParseConfig(XAMIOTrace trace, XAMUmpClientItem item, XAMDriverPropertiesExtended config, out int? bitindex,out string complexPath)
        {
            if(item == null)
                item = new XAMUmpClientItem();

            item.ActorId = System.Convert.ToInt32(config.ParameterToks[0]);
            item.MessageId = (UmpMessageID)System.Convert.ToInt32(config.ParameterToks[1]);

            switch(item.MessageId)
            {
                case UmpMessageID.EditValue:
                case UmpMessageID.DateTime:
                case UmpMessageID.I2C_Temperature:
                    break;
                default:
                    throw new NotImplementedException("Message ID <" + item.MessageId + "> not supported");
            }

            item.Intervall = System.Convert.ToInt32(config.GetOrDefault(6, "0"));
            item.OffsetWriteValue = config.GetOrDefaultDouble(7, 0.0);
            item.IsSendOnREOnly = System.Convert.ToBoolean(config.GetOrDefault(8, "false"));

            bitindex = null;
            complexPath = null;
            return item;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XAMCommon.Trace;
using XAMIO.Ulux.Ump.Telegram;

namespace XAMIO.Ulux.Ump.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class XAMUmpVideoState
    {
        public static XAMUmpMessageTelegram Create( TraceDelegate trace)
        {
            return new XAMUmpMessageTelegram(UmpMessageID.VideoState, 0, new byte[0], trace);
        }
       
    }
}

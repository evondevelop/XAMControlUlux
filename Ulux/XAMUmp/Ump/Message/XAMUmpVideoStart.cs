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
    public class XAMUmpVideoStart
    {


        public static XAMUmpMessageTelegram Create(int sequenceID, TraceDelegate trace)
        {
            System.Threading.Thread.Sleep(1000);
           
            List<byte> d = new List<byte>();
            d.AddRange(BitConverter.GetBytes(0));
            d.AddRange(BitConverter.GetBytes(sequenceID));

            // set audio play remote
            return new XAMUmpMessageTelegram(UmpMessageID.VideoStart, 0, d.ToArray(), trace);
        }
    }
}

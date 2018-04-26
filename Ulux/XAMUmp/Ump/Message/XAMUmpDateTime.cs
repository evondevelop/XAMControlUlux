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
    public class XAMUmpDateTime
    {


        /// <summary>
        /// Creates the specified dt.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="trace">The trace.</param>
        /// <returns></returns>
        public static XAMUmpMessageTelegram Create(DateTime dt, TraceDelegate trace)
        {
            trace("datetime!", TracePrio.MESSAGE);
            List<byte> sndDatetime = new List<byte>();
            sndDatetime.Add((byte)dt.Second);
            sndDatetime.Add((byte)dt.Minute);
            sndDatetime.Add((byte)dt.Hour);
            sndDatetime.Add((byte)dt.DayOfWeek);
            sndDatetime.Add((byte)dt.Day);
            sndDatetime.Add((byte)dt.Month);
            sndDatetime.Add((byte)(dt.Year & 0xFF));
            sndDatetime.Add((byte)((dt.Year >> 8) & 0xFF));

            // set audio play remote
            return new XAMUmpMessageTelegram(UmpMessageID.DateTime, 0, sndDatetime.ToArray(), trace);
        }                
    }
}

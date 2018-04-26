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
    public class XAMUmpAudioPlayRemote
    {
        public static XAMUmpMessageTelegram Create(byte volume, Equalizer eq, short sequenceID, TraceDelegate trace)
        {
            return Create(volume, eq, new XAMUmpPlayFlags() { IgnoreAMSNetID = true }, 0, sequenceID, 882, 20000, "255.255.255.255", "255.255.255.255", trace);
        }
        public static XAMUmpMessageTelegram Create(byte volume, Equalizer eq, short sequenceID, string AMSNetID, TraceDelegate trace)
        {
            return Create(volume, eq, new XAMUmpPlayFlags() { IgnoreAMSNetID = true }, 0, sequenceID, 882, 20000, "255.255.255.255", AMSNetID, trace);
        }

        public static XAMUmpMessageTelegram Create(byte volume, Equalizer eq, XAMUmpPlayFlags playFlags, short incVolumeTime, short sequenceID, short bytesPerFrame, int delayBetweenFrames, string IpAdr, string AMSNetIp, TraceDelegate trace)
        {
            List<byte> d = new List<byte>();
            if (volume > 100)
                volume = 100;
            d.Add((byte)volume); // Volume 100 = maximum
            d.Add((byte)eq); // equalizer 0 = normal
            byte[] pf = playFlags.GetBytes();
            d.Add(pf[0]); // playfalsgs 2  00001  = ignore AMSNetID
            d.Add(pf[1]); // playfalsgs 0
            d.AddRange(BitConverter.GetBytes(incVolumeTime)); // inc volume time
            d.AddRange(BitConverter.GetBytes(sequenceID));
            d.AddRange(BitConverter.GetBytes(bytesPerFrame));// bytes per frame 882 = default
            d.Add((byte)0); // reserved
            d.Add((byte)0); // reserved
            d.AddRange(BitConverter.GetBytes((Int32)delayBetweenFrames)); // delay between frames
            byte[] ip = IPAddress.Parse(IpAdr).GetAddressBytes();
            d.Add(ip[0]); // ip address // 255.255.255.255 = exapt all
            d.Add(ip[1]); // ip address // 255.255.255.255 = exapt all
            d.Add(ip[2]); // ip address // 255.255.255.255 = exapt all
            d.Add(ip[3]); // ip address // 255.255.255.255 = exapt all

            byte[] ams = IPAddress.Parse(AMSNetIp).GetAddressBytes();
            d.Add(ams[0]); // ip address // 255.255.255.255 = exapt all
            d.Add(ams[1]); // ip address // 255.255.255.255 = exapt all
            d.Add(ams[2]); // ip address // 255.255.255.255 = exapt all
            d.Add(ams[3]); // ip address // 255.255.255.255 = exapt all
            d.Add((byte)1); // AMSnetID
            d.Add((byte)1); // AMSnetID

            d.Add((byte)0); // reserved
            d.Add((byte)0); // reserved

            // set audio play remote
            return new XAMUmpMessageTelegram(UmpMessageID.AudioPlayRemote, 0, d.ToArray(), trace);
        }
    }
}

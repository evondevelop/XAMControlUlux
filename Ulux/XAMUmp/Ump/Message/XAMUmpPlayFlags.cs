using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAMIO.Ulux.Ump.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class XAMUmpPlayFlags
    {
        /// <summary>
        /// The alarm
        /// </summary>
        public bool Alarm;
        /// <summary>
        /// The ignore ams net identifier
        /// </summary>
        public bool IgnoreAMSNetID;
        /// <summary>
        /// The no audio page
        /// </summary>
        public bool NoAudioPage;
        /// <summary>
        /// The dont change volume
        /// </summary>
        public bool DontChangeVolume;
        /// <summary>
        /// The increment volume
        /// </summary>
        public bool IncrementVolume;

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] b = new byte[2];

            if (Alarm)
                b[0] |= 0x01;
            if (IgnoreAMSNetID)
                b[0] |= 0x02;
            if (NoAudioPage)
                b[0] |= 0x08;
            if (DontChangeVolume)
                b[0] |= 0x10;
            if (IncrementVolume)
                b[0] |= 0x20;

            return b;
        }
    }
}

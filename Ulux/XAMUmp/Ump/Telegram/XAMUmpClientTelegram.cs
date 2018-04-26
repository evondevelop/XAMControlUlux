using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XAMIO.Common.Net;
using XAMCommon.Trace;
using XAMIO.Common;
using XAMCommon.Base;

namespace XAMIO.Ulux.Ump.Telegram
{
    public class XAMUmpTelegram : TelegramBase
    {
        private int FrameID;

        private int FrameLength;
        private int FrameVersion;
        public int PackageID { get; set; }
        public int ProjectID { get; set; }
        public int FirmwareVersion { get; set; }
        public int SwitchId { get; set; }
        public int DesignId { get; set; }

        public List<XAMUmpMessageTelegram> Messages { get; set; }

        public XAMUmpVideoStreamTelegram VideoStream { get; set; }
        public bool IsVideoStreamAck = false;
        //public XAMUmpAudioStreamTelegram AudioStream { get; set; }

        
                
        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpTelegram"/> class.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public XAMUmpTelegram()
            : base()
        {
            FrameID = 0x8601;
            FrameVersion = 0x0201;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpTelegram"/> class.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public XAMUmpTelegram(TraceDelegate trace)
            : base(trace)
        {
            FrameID = 0x8601;
            FrameVersion = 0x0201;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpTelegram"/> class.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public XAMUmpTelegram(int packageID, int projectID,int firmwareVersion,int switchId,int designId, TraceDelegate trace)
            : base(trace)
        {
            FrameID = 0x8601;
            FrameVersion = 0x0220;
            PackageID = packageID;
            ProjectID = projectID;
            FirmwareVersion = firmwareVersion;
            SwitchId = switchId;
            DesignId = designId;
        }
        

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encodes this instance.
        /// </summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            FrameLength = 16;

            //if (AudioStream != null)
            //{
            //    return AudioStream.Encode();
            //}

            if (VideoStream != null)
            {
                FrameLength += VideoStream.Length;
                FrameID = 0x8602;
            }
            else
            {
                FrameID = 0x8601;
                foreach (var msg in Messages)
                {
                    FrameLength += msg.Length;
                }
            }

            List<byte> data = new List<byte>();
            AddInt16(ref data, FrameID);
            AddInt16(ref data, FrameLength);
            AddInt16(ref data, FrameVersion);
            AddInt16(ref data, PackageID);
            AddInt16(ref data, ProjectID);
            AddInt16(ref data, FirmwareVersion);
            AddInt16(ref data, SwitchId);
            AddInt16(ref data, DesignId);

            if (VideoStream != null)
            {
                data.AddRange(VideoStream.Encode());
            }
            else
            {
                foreach (var msg in Messages)
                {
                    data.AddRange(msg.Encode());
                }
            }
           
            return data.ToArray();
        }

        private void AddInt16(ref List<byte> data, int val)
        {
            data.Add((byte)(val & 0xFF));
            data.Add((byte)((val >> 8) & 0xFF));
        }

        /// <summary>
        /// Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="XAMIO.Common.Net.FrameToLessDataException">
        /// </exception>
        /// <exception cref="XAMIO.Common.Net.TelegramDecodeException">
        /// Wrong Start Byte
        /// or
        /// Wrong Command
        /// </exception>
        public override void Decode(byte[] data)
        {
            if (data.Length < 16)
                throw new FrameToLessDataException();


            FrameID = BitConverter.ToUInt16(data, 0); //  BigEndian.ToInt16(data, 0, true);

            FrameLength = BitConverter.ToUInt16(data, 2); // BigEndian.ToInt16(data, 2, true);
            FrameVersion = BitConverter.ToUInt16(data, 4); // BigEndian.ToInt16(data, 4, true);
            PackageID = BitConverter.ToUInt16(data, 6);//  BigEndian.ToInt16(data, 6, true);
            ProjectID = BitConverter.ToUInt16(data, 8); // BigEndian.ToInt16(data, 8, true);
            FirmwareVersion = BitConverter.ToUInt16(data, 10); //  BigEndian.ToInt16(data, 10, true);
            SwitchId = BitConverter.ToUInt16(data, 12); // BigEndian.ToInt16(data, 12, true);
            DesignId = BitConverter.ToUInt16(data, 14); // BigEndian.ToInt16(data, 14, true);

            if (FrameLength > data.Length)
                throw new FrameToLessDataException();

            if (this.FrameID == 0x8601)
            {
                int offset = 16;
                int msglen = FrameLength - offset;

                Messages = new List<XAMUmpMessageTelegram>();
                while (msglen > 0)
                {
                    byte[] msgbytes = new byte[msglen];
                    Array.Copy(data, offset, msgbytes, 0, msglen);

                    var msg = new XAMUmpMessageTelegram(base.Trace);
                    msg.Decode(msgbytes);
                    Messages.Add(msg);

                    offset += msg.Length;
                    msglen = FrameLength - offset;
                }
            }
            else if(this.FrameID == 0x8602)
            {
                this.VideoStream = new XAMUmpVideoStreamTelegram(this.Trace);

                if (data.Length == 16)
                {
                    this.IsVideoStreamAck = true;
                }
                else
                {
                    this.VideoStream.Decode(data.SubArray(16));
                }
            }
            else
            {
                throw new TelegramDecodeException("Unknown FrameID " + this.FrameID);
            }
        }
    }
}

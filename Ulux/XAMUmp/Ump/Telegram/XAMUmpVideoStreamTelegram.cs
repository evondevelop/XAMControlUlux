using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XAMIO.Common.Net;
using XAMCommon.Trace;
using XAMIO.Common;
using XAMIO.Ulux.Ump.Message;
using XAMCommon.Base;

namespace XAMIO.Ulux.Ump.Telegram
{
    public class XAMUmpVideoStreamTelegram : TelegramBase
    {
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public short Length { get { return (short)(12 + VideoData.Length); } }

        /// <summary>
        /// Gets or sets the stream flags.
        /// </summary>
        /// <value>
        /// The stream flags.
        /// </value>
        public XAMUmpStreamFlags StreamFlags { get; set; }

        /// <summary>
        /// Gets or sets the start line.
        /// </summary>
        /// <value>
        /// The start line.
        /// </value>
        public short StartLine { get; set; }
        /// <summary>
        /// Gets or sets the line count.
        /// </summary>
        /// <value>
        /// The line count.
        /// </value>
        public short LineCount { get; set; }


        /// <summary>
        /// Gets or sets the sequence identifier.
        /// </summary>
        /// <value>
        /// The sequence identifier.
        /// </value>
        public int SequenceID { get; set; }

        /// <summary>
        /// Gets or sets the video data.
        /// </summary>
        /// <value>
        /// The video data.
        /// </value>
        public byte[] VideoData { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpTelegram"/> class.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public XAMUmpVideoStreamTelegram()
            : base()
        {
            StreamFlags = new XAMUmpStreamFlags();
            SequenceID = 0;
            StartLine = 0;
            LineCount = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpTelegram"/> class.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public XAMUmpVideoStreamTelegram(TraceDelegate trace)
            : base(trace)
        {
            StreamFlags = new XAMUmpStreamFlags();
            SequenceID = 0;
            StartLine = 0;
            LineCount = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpTelegram"/> class.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public XAMUmpVideoStreamTelegram(XAMUmpStreamFlags streamFlags, int sequenceID, short startLine, short lineCount, byte[] videoData, TraceDelegate trace)
            : base(trace)
        {
            StreamFlags = streamFlags;
            SequenceID = sequenceID;
            StartLine = startLine;
            LineCount = lineCount;
            VideoData = videoData;
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
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

            List<byte> data = new List<byte>();
            data.AddRange(StreamFlags.Bytes);

            data.Add((byte)(SequenceID & 0xFF));
            data.Add((byte)((SequenceID >> 8) & 0xFF));
            data.Add((byte)((SequenceID >> 16) & 0xFF));
            data.Add((byte)((SequenceID >> 24) & 0xFF));

            data.Add((byte)(StartLine & 0xFF));
            data.Add((byte)((StartLine >> 8) & 0xFF));

            data.Add((byte)(LineCount & 0xFF));
            data.Add((byte)((LineCount >> 8) & 0xFF));
            data.AddRange(VideoData);
            return data.ToArray();
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
            this.StreamFlags = new XAMUmpStreamFlags();
            if ((data[0] & 0x01) > 0)
                this.StreamFlags.Acknowledge = true;

            this.SequenceID = (int)BitConverter.ToUInt32(data, 4);
            this.StartLine = (short)BitConverter.ToUInt16(data, 8);
            this.LineCount = (short)BitConverter.ToUInt16(data, 10);
            this.VideoData = data.SubArray(12);
        }
    }
}

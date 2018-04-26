using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XAMIO.Common.Net;
using XAMCommon.Trace;
using XAMIO.Common;

namespace XAMIO.Ulux.Ump.Telegram
{

    public class XAMUmpMessageTelegram : TelegramBase
    {
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public byte Length { get { return (byte)(4 + Value.Length); } }

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public UmpMessageID MessageID { get; set; }

        /// <summary>
        /// Gets or sets the actor identifier.
        /// </summary>
        /// <value>
        /// The actor identifier.
        /// </value>
        public int ActorID { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public byte[] Value { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpTelegram"/> class.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public XAMUmpMessageTelegram()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpTelegram"/> class.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public XAMUmpMessageTelegram(TraceDelegate trace)
            : base(trace)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpTelegram"/> class.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public XAMUmpMessageTelegram(UmpMessageID messageID, int actorID, byte[] value, TraceDelegate trace)
            : base(trace)
        {
            this.MessageID = messageID;
            this.ActorID = actorID;
            this.Value = value;
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
            data.Add(Length);
            data.Add((byte)MessageID);
            data.Add((byte)(ActorID & 0xFF));
            data.Add((byte)((ActorID >> 8) & 0xFF));
            data.AddRange(Value);
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
            if (data.Length < 4)
                throw new FrameToLessDataException();
            int len = data[0];
            MessageID = (UmpMessageID)data[1];
            ActorID = BitConverter.ToUInt16(data, 2); // BigEndian.ToInt16(data, 2, true);

            if (len > data.Length)
                throw new FrameToLessDataException();

            int valueLen = len - 4;
            Value = new byte[valueLen];
            Array.Copy(data, 4, Value, 0, valueLen);
        }
    }
}

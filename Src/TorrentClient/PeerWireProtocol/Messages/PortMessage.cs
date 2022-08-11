using System;
using System.Net;
using System.Text;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// The port message.
    /// </summary>
    public class PortMessage : PeerMessage
    {
        #region Public Fields

        /// <summary>
        /// The message unique identifier.
        /// </summary>
        public const byte MessageId = 9;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The message unique identifier length in bytes.
        /// </summary>
        private const int MessageIdLength = 1;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLength = 3;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLengthLength = 4;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int PayloadLength = 2;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortMessage"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public PortMessage(ushort port)
        {
            this.Port = port;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="PortMessage"/> class from being created.
        /// </summary>
        private PortMessage()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the length in bytes.
        /// </summary>
        /// <value>
        /// The length in bytes.
        /// </value>
        public override int Length
        {
            get
            {
                return MessageLengthLength + MessageIdLength + PayloadLength;
            }
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public ushort Port
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offsetFrom">The offset.</param>
        /// <param name="offsetTo">The offset to.</param>
        /// <returns>
        /// True if decoding was successful; false otherwise.
        /// </returns>
        public static PortMessage TryDecode(byte[] buffer, int offsetFrom, int offsetTo)
        {
            int messageLength;
            byte messageId;
            ushort port;
            int offsetStart = offsetFrom;
            PortMessage message = new PortMessage();

            if (buffer != null &&
                buffer.Length >= offsetFrom + MessageLengthLength + MessageIdLength + PayloadLength &&
                offsetFrom >= 0 &&
                offsetTo >= offsetFrom &&
                offsetTo <= buffer.Length)
            {
                messageLength = Message.ReadInt(buffer, offsetStart);
                offsetStart += Message.IntLength;
                messageId = Message.ReadByte(buffer, offsetStart);
                offsetStart++;
                port = (ushort)Message.ReadShort(buffer, offsetStart);
                offsetStart += Message.ShortLength;

                if (messageLength == MessageLength &&
                    messageId == MessageId &&
                    port >= IPEndPoint.MinPort &&
                    port <= IPEndPoint.MaxPort)
                {
                    if (offsetFrom <= offsetTo)
                    {
                        message = new PortMessage(port);
                    }
                    else
                    {
                        message.IsIncomplete = true;
                    }
                }
            }

            return message;
        }

        /// <summary>
        /// Encodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>
        /// The encoded peer message.
        /// </returns>
        public override int Encode(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length}");

            int written = offset;

            written += Message.Write(buffer, written, MessageLength);
            written += Message.Write(buffer, written, MessageId);
            written += Message.Write(buffer, written, this.Port);

            return this.CheckWritten(written - offset);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            PortMessage msg = obj as PortMessage;

            return msg == null ? false : this.Port == msg.Port;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Port.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"PortMessage: Port = {this.Port}";
        }

        #endregion Public Methods
    }
}

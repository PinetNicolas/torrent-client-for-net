using System;
using System.Globalization;
using System.Text;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// The request message.
    /// </summary>
    public class RequestMessage : PeerMessage
    {
        #region Public Fields

        /// <summary>
        /// The message unique identifier.
        /// </summary>
        public const byte MessageId = 6;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The block length length in bytes.
        /// </summary>
        private const int BlockLengthLength = 4;

        /// <summary>
        /// The block offset length in bytes.
        /// </summary>
        private const int BlockOffsetLength = 4;

        /// <summary>
        /// The message unique identifier length in bytes.
        /// </summary>
        private const int MessageIdLength = 1;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLength = 13;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLengthLength = 4;

        /// <summary>
        /// The piece index length in bytes.
        /// </summary>
        private const int PieceIndexLength = 4;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage" /> class.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <param name="blockOffset">The block offset.</param>
        /// <param name="blockLength">Length of the block.</param>
        public RequestMessage(int pieceIndex, int blockOffset, int blockLength)
        {
            this.PieceIndex = pieceIndex;
            this.BlockOffset = blockOffset;
            this.BlockLength = blockLength;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="RequestMessage"/> class from being created.
        /// </summary>
        private RequestMessage()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the length of the block.
        /// </summary>
        /// <value>
        /// The length of the block.
        /// </value>
        public int BlockLength
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the block offset.
        /// </summary>
        /// <value>
        /// The block offset.
        /// </value>
        public int BlockOffset
        {
            get;
            private set;
        }

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
                return MessageLengthLength + MessageIdLength + PieceIndexLength + BlockOffsetLength + BlockLengthLength;
            }
        }

        /// <summary>
        /// Gets the index of the piece.
        /// </summary>
        /// <value>
        /// The index of the piece.
        /// </value>
        public int PieceIndex
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
        public static RequestMessage TryDecode(byte[] buffer, int offsetFrom, int offsetTo)
        {
            int messageLength;
            byte messageId;
            int pieceIndex;
            int blockOffset;
            int blockLength;

            RequestMessage message = new RequestMessage();

            if (buffer != null &&
                buffer.Length >= offsetFrom + MessageLengthLength + MessageIdLength + PieceIndexLength + BlockOffsetLength + BlockLengthLength &&
                offsetFrom >= 0 &&
                offsetTo >= offsetFrom &&
                offsetTo <= buffer.Length)
            {
                messageLength = Message.ReadInt(buffer, offsetFrom);
                offsetFrom += Message.IntLength;
                messageId = Message.ReadByte(buffer, offsetFrom);
                offsetFrom++;
                pieceIndex = Message.ReadInt(buffer, offsetFrom);
                offsetFrom += Message.IntLength;
                blockOffset = Message.ReadInt(buffer, offsetFrom);
                offsetFrom += Message.IntLength;
                blockLength = Message.ReadInt(buffer, offsetFrom);
                offsetFrom += Message.IntLength;

                if (messageLength == MessageIdLength + PieceIndexLength + BlockOffsetLength + BlockLengthLength &&
                    messageId == MessageId &&
                    pieceIndex >= 0 &&
                    blockOffset >= 0 &&
                    blockLength >= 0)
                {
                    if (offsetFrom <= offsetTo)
                    {
                        message = new RequestMessage(pieceIndex, blockOffset, blockLength);
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

            if(offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset",$"Offset must be between 0 and {buffer.Length}");

            int written = offset;

            written+= Message.Write(buffer, written, MessageLength);
            written += Message.Write(buffer, written, MessageId);
            written += Message.Write(buffer, written, this.PieceIndex);
            written += Message.Write(buffer, written, this.BlockOffset);
            written += Message.Write(buffer, written, this.BlockLength);

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
            RequestMessage msg = obj as RequestMessage;

            return msg == null ? false : this.PieceIndex == msg.PieceIndex &&
                                         this.BlockOffset == msg.BlockOffset &&
                                         this.BlockLength == msg.BlockLength;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.PieceIndex.GetHashCode() ^
                   this.BlockLength.GetHashCode() ^
                   this.BlockOffset.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb;

            sb = new System.Text.StringBuilder();
            sb.Append("RequestMessage: ");
            sb.AppendFormat(CultureInfo.InvariantCulture, "PieceIndex = {0}", this.PieceIndex);
            sb.AppendFormat(CultureInfo.InvariantCulture, "BlockOffset = {0}", this.BlockOffset);
            sb.AppendFormat(CultureInfo.InvariantCulture, "BlockLength = {0}", this.BlockLength);

            return sb.ToString();
        }

        #endregion Public Methods
    }
}

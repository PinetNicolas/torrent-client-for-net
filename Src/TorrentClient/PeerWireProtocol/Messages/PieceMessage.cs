using System;
using System.Text;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// The piece message.
    /// </summary>
    public class PieceMessage : PeerMessage
    {
        #region Public Fields

        /// <summary>
        /// The message unique identifier.
        /// </summary>
        public const byte MessageId = 7;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The block offset length.
        /// </summary>
        private const int BlockOffsetLength = 4;

        /// <summary>
        /// The message unique identifier length in bytes.
        /// </summary>
        private const int MessageIdLength = 1;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLengthLength = 4;

        /// <summary>
        /// The piece index length.
        /// </summary>
        private const int PieiceIndexLength = 4;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PieceMessage" /> class.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <param name="blockOffset">The block offset.</param>
        /// <param name="blockDataLength">Length of the block data.</param>
        /// <param name="data">The block data.</param>
        public PieceMessage(int pieceIndex, int blockOffset, int blockDataLength, byte[] data)
        {
            this.PieceIndex = pieceIndex;
            this.BlockOffset = blockOffset;
            this.BlockDataLength = blockDataLength;
            this.Data = data;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="PieceMessage"/> class from being created.
        /// </summary>
        private PieceMessage()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the length of the block data.
        /// </summary>
        /// <value>
        /// The length of the block data.
        /// </value>
        public int BlockDataLength
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
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public byte[] Data
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
                return MessageLengthLength + MessageIdLength + PieiceIndexLength + BlockOffsetLength + this.BlockDataLength;
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
        /// <param name="offsetFrom">The offset from.</param>
        /// <param name="offsetTo">The offset to.</param>
        /// <param name="message">The message.</param>
        /// <param name="isIncomplete">if set to <c>true</c> the message is incomplete.</param>
        /// <param name="destination">The destination data array.</param>
        /// <returns>
        /// True if decoding was successful; false otherwise.
        /// </returns>
        public static PieceMessage TryDecode(byte[] buffer, int offsetFrom, int offsetTo, byte[] destination = null)
        {
            int messageLength;
            byte messageId;
            int pieceIndex;
            int blockOffset;
            int blockDataLength = 0;
            int destinationOffset = 0;
            int offsetFrom2 = offsetFrom;

            PieceMessage message = new PieceMessage();

            if (buffer != null &&
                buffer.Length > offsetFrom2 + MessageLengthLength + MessageIdLength + PieiceIndexLength + BlockOffsetLength &&
                offsetFrom2 >= 0 &&
                offsetFrom2 < buffer.Length &&
                offsetTo >= offsetFrom2 &&
                offsetTo <= buffer.Length)
            {
                messageLength = Message.ReadInt(buffer, offsetFrom2);
                offsetFrom2 += Message.IntLength;
                messageId = Message.ReadByte(buffer, offsetFrom2);
                offsetFrom2++;
                pieceIndex = Message.ReadInt(buffer, offsetFrom2);
                offsetFrom2 += Message.IntLength;
                blockOffset = Message.ReadInt(buffer, offsetFrom2);
                offsetFrom2 += Message.IntLength;
                blockDataLength = messageLength - MessageIdLength - PieiceIndexLength - BlockOffsetLength;

                if (messageLength > MessageIdLength + PieiceIndexLength + BlockOffsetLength &&
                    messageId == MessageId &&
                    pieceIndex >= 0 &&
                    blockOffset >= 0 &&
                    blockDataLength >= 0)
                {
                    if (offsetFrom2 + blockDataLength <= offsetTo)
                    {
                        if (destination == null)
                        {
                            destination = new byte[blockDataLength];
                            destinationOffset = 0;
                        }
                        else
                        {
                            destinationOffset = blockOffset;
                        }

                        Message.Copy(buffer, offsetFrom2, destination, destinationOffset, blockDataLength);

                        message = new PieceMessage(pieceIndex, blockOffset, blockDataLength, destination);
                        offsetFrom = offsetFrom2 + blockDataLength;
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

            written += Message.Write(buffer, written, MessageIdLength + PieiceIndexLength + BlockOffsetLength + this.Data.Length);
            written += Message.Write(buffer, written, MessageId);
            written += Message.Write(buffer, written, this.PieceIndex);
            written += Message.Write(buffer, written, this.BlockOffset);
            written += Message.Write(buffer, written, this.Data);

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
            PieceMessage msg = obj as PieceMessage;

            if (msg == null)
            {
                return false;
            }
            else if (this.PieceIndex == msg.PieceIndex &&
                     this.BlockOffset == msg.BlockOffset &&
                     this.Data.ToHexaDecimalString() == msg.Data.ToHexaDecimalString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            int hash;

            hash = this.PieceIndex.GetHashCode() ^
                   this.BlockOffset.GetHashCode() ^
                   this.Data.ToHexaDecimalString().GetHashCode(StringComparison.InvariantCulture);

            return hash;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"PieceMessage: PieceIndex = {this.PieceIndex}, BlockOffset = {this.BlockOffset}, BlockData = byte[{this.Data.Length}]";
        }

        #endregion Public Methods
    }
}

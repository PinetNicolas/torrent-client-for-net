using System.Collections.Generic;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// The peer wire protocol message base class.
    /// </summary>
    public abstract class PeerMessage : Message
    {
        #region Public Fields

        /// <summary>
        /// The default block length in bytes (16kB)
        /// </summary>
        public const int DefaultBlockLength = 16384;

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        /// Decodes the messages in the buffer.
        /// </summary>
        /// <param name="buffer">The data.</param>
        /// <param name="offsetStart">The offset.</param>
        /// <param name="offsetEnd">The offset end.</param>
        /// <returns>
        /// The list of messages.
        /// </returns>
        public static IEnumerable<PeerMessage> Decode(byte[] buffer, int offsetStart, int offsetEnd)
        {
            List<PeerMessage> messages = new List<PeerMessage>();
            PeerMessage message;
            int offset = offsetStart;

            // walk through the array and try to decode messages
            while (offset <= offsetEnd)
            {
                message = TryDecodeMessage(buffer, offset, offsetEnd);
                if (!message.IsIncomplete)
                {
                    // successfully decoded message
                    messages.Add(message);

                    // remember where we left off
                    offset += message.Length;
                }
                else if (message.IsIncomplete)
                {
                    // message of variable length is present but incomplete -> stop advancing
                    break;
                }
                else
                {
                    // move to next byte
                    offset++;
                }
            }

            return messages;
        }

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offsetFrom">The offset from.</param>
        /// <param name="offsetTo">The offset to.</param>
        /// <returns>
        /// True if decoding was successful; false otherwise.
        /// </returns>
        public static PeerMessage TryDecodeMessage(byte[] buffer, int offsetFrom, int offsetTo)
        {
            byte messageId;
            int messageLength;
            int offset2 = offsetFrom;

            PeerMessage message = null;

            if (buffer !=null && buffer.Length > 0 &&
                buffer.Length >= offsetFrom + Message.IntLength + Message.ByteLength)
            {
                messageLength = Message.ReadInt(buffer, offset2);
                offset2 += Message.IntLength;
                messageId = Message.ReadByte(buffer, offset2);
                offset2++;
                offset2 = offsetFrom; // reset offset


                if (messageId == ChokeMessage.MessageId)
                {
                    ChokeMessage message2 = ChokeMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
                else if (messageId == UnchokeMessage.MessageId)
                {
                    UnchokeMessage message2 = UnchokeMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
                else if (messageId == InterestedMessage.MessageId)
                {
                    InterestedMessage message2 = InterestedMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
                else if (messageId == UninterestedMessage.MessageId)
                {
                    UninterestedMessage message2 = UninterestedMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
                else if (messageId == HaveMessage.MessageId)
                {
                    HaveMessage message2 = HaveMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
                else if (messageId == BitFieldMessage.MessageId)
                {
                    BitFieldMessage message2 = BitFieldMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
                else if (messageId == RequestMessage.MessageId)
                {
                    RequestMessage message2 = RequestMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
                else if (messageId == CancelMessage.MessageId)
                {
                    CancelMessage message2 = CancelMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
                else if (messageId == PortMessage.MessageId)
                {
                    PortMessage message2 = PortMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
                else if (messageId == PieceMessage.MessageId)
                {
                    PieceMessage message2 = PieceMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
                else
                {
                    HandshakeMessage message2 = HandshakeMessage.TryDecode(buffer, offset2, offsetTo);
                    offset2 += message2.Length;
                    message = message2;
                }
            }

            return message;
        }

        #endregion Public Methods
    }
}

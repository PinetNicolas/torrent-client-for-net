using System;
using System.Text;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The UDP error message.
    /// </summary>
    public class ErrorMessage : TrackerMessage
    {
        #region Private Fields

        /// <summary>
        /// The action length in bytes.
        /// </summary>
        private const int ActionLength = 4;

        /// <summary>
        /// The transaction identifier length in bytes.
        /// </summary>
        private const int TransactionIdLength = 4;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
        /// </summary>
        /// <param name="transactionId">The transaction unique identifier.</param>
        /// <param name="errorMessage">The error.</param>
        public ErrorMessage(int transactionId, string errorMessage)
            : base(TrackingAction.Error, transactionId)
        {
            this.ErrorText = errorMessage;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the error text.
        /// </summary>
        /// <value>
        /// The error text.
        /// </value>
        public string ErrorText
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
                return ActionLength + TransactionIdLength + Encoding.ASCII.GetByteCount(this.ErrorText);
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>
        /// The message decode or null if problem
        /// </returns>
        public static ErrorMessage TryDecode(byte[] buffer, int offset)
        {
            int action;
            int transactionId;
            string errorMessage;

            ErrorMessage message = null;

            if (buffer != null &&
                buffer.Length >= offset + ActionLength + TransactionIdLength &&
                offset >= 0)
            {
                action = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                transactionId = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;

                if (action == (int)TrackingAction.Error &&
                    transactionId >= 0)
                {
                    errorMessage = Message.ReadString(buffer, offset, buffer.Length - offset);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        message = new ErrorMessage(transactionId, errorMessage);
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
        /// <returns>The number of bytes written.</returns>
        public override int Encode(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length}");

            int written = offset;

            written += Message.Write(buffer, written, (int)this.Action);
            written += Message.Write(buffer, written, this.TransactionId);
            written += Message.Write(buffer, written, this.ErrorText);

            return written - offset;
        }

        #endregion Public Methods
    }
}

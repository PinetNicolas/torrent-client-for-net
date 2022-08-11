using System;
using System.Collections.Generic;
using System.Linq;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The scrape message.
    /// </summary>
    public class ScrapeMessage : TrackerMessage
    {
        #region Private Fields

        /// <summary>
        /// The action length in bytes.
        /// </summary>
        private const int ActionLength = 4;

        /// <summary>
        /// The connection identifier length in bytes.
        /// </summary>
        private const int ConnectionIdLength = 8;

        /// <summary>
        /// The information hash length in bytes.
        /// </summary>
        private const int InfoHashLength = 20;

        /// <summary>
        /// The transaction identifier length in bytes.
        /// </summary>
        private const int TransactionIdLength = 4;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeMessage" /> class.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="transactionId">The transaction unique identifier.</param>
        /// <param name="infoHashes">The info hashes.</param>
        public ScrapeMessage(long connectionId, int transactionId, IEnumerable<string> infoHashes)
            : base(TrackingAction.Scrape, transactionId)
        {
            this.ConnectionId = connectionId;
            this.InfoHashes = infoHashes;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the connection identifier.
        /// </summary>
        /// <value>
        /// The connection identifier.
        /// </value>
        public long ConnectionId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the information hashes.
        /// </summary>
        /// <value>
        /// The information hashes.
        /// </value>
        public IEnumerable<string> InfoHashes
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
                return ConnectionIdLength + ActionLength + TransactionIdLength + (this.InfoHashes.Count() * 20);
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
        public static ScrapeMessage TryDecode(byte[] buffer, int offset)
        {
            long connectionId;
            int action;
            int transactionId;
            List<string> infoHashes = new List<string>();

            ScrapeMessage message = null;

            if (buffer != null &&
                buffer.Length >= offset + ConnectionIdLength + ActionLength + TransactionIdLength &&
                offset >= 0)
            {
                connectionId = Message.ReadLong(buffer, offset);
                offset += Message.LongLength;
                action = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                transactionId = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;

                if (connectionId >= 0 &&
                    action == (int)TrackingAction.Scrape &&
                    transactionId >= 0)
                {
                    while (offset <= buffer.Length - InfoHashLength)
                    {
                        infoHashes.Add(Message.ReadBytes(buffer, offset, InfoHashLength).ToHexaDecimalString());
                        offset += InfoHashLength;
                    }

                    message = new ScrapeMessage(connectionId, transactionId, infoHashes);
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

            written += Message.Write(buffer, written, this.ConnectionId);
            written += Message.Write(buffer, written, (int)this.Action);
            written += Message.Write(buffer, written, this.TransactionId);

            foreach (string infoHash in this.InfoHashes)
            {
                written += Message.Write(buffer, written, infoHash.ToByteArray());
            }

            return written - offset;
        }

        #endregion Public Methods
    }
}

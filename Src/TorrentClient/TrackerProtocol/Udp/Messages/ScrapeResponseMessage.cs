using System;
using System.Collections.Generic;
using System.Linq;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The scrape response message.
    /// </summary>
    public sealed class ScrapeResponseMessage : TrackerMessage
    {
        #region Private Fields

        /// <summary>
        /// The action length in bytes.
        /// </summary>
        private const int ActionLength = 4;

        /// <summary>
        /// The completed length in bytes.
        /// </summary>
        private const int CompletedLength = 4;

        /// <summary>
        /// The leechers length in bytes.
        /// </summary>
        private const int LeechersLength = 4;

        /// <summary>
        /// The seeders length in bytes.
        /// </summary>
        private const int SeedersLength = 4;

        /// <summary>
        /// The transaction identifier length in bytes.
        /// </summary>
        private const int TransactionIdLength = 4;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeResponseMessage"/> class.
        /// </summary>
        public ScrapeResponseMessage()
            : this(0, new List<ScrapeDetails>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeResponseMessage"/> class.
        /// </summary>
        /// <param name="transactionId">The transaction unique identifier.</param>
        /// <param name="scrapes">The scrapes.</param>
        public ScrapeResponseMessage(int transactionId, IEnumerable<ScrapeDetails> scrapes)
            : base(TrackingAction.Scrape, transactionId)
        {
            this.Scrapes = scrapes;
        }

        #endregion Public Constructors

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
                return ActionLength + TransactionIdLength + (this.Scrapes.Count() * (SeedersLength + CompletedLength + LeechersLength));
            }
        }

        /// <summary>
        /// Gets the scrapes.
        /// </summary>
        /// <value>
        /// The scrapes.
        /// </value>
        public IEnumerable<ScrapeDetails> Scrapes
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
        /// <param name="offset">The offset.</param>
        /// <returns>
        /// The message decode or null if problem
        /// </returns>
        public static ScrapeResponseMessage TryDecode(byte[] buffer, int offset)
        {
            int action;
            int transactionId;
            int seeds;
            int completed;
            int leechers;
            List<ScrapeDetails> scrapeInfo = new List<ScrapeDetails>();

            ScrapeResponseMessage message = null;

            if (buffer != null &&
                buffer.Length >= offset + ActionLength + TransactionIdLength &&
                offset >= 0)
            {
                action = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                transactionId = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;

                if (action == (int)TrackingAction.Scrape &&
                    transactionId >= 0)
                {
                    while (offset <= buffer.Length - SeedersLength - CompletedLength - LeechersLength)
                    {
                        seeds = Message.ReadInt(buffer, offset);
                        offset += Message.IntLength;
                        completed = Message.ReadInt(buffer, offset);
                        offset += Message.IntLength;
                        leechers = Message.ReadInt(buffer, offset);
                        offset += Message.IntLength;

                        scrapeInfo.Add(new ScrapeDetails(seeds, leechers, completed));
                    }

                    message = new ScrapeResponseMessage(transactionId, scrapeInfo);
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

            foreach (var scrape in this.Scrapes)
            {
                written += Message.Write(buffer, written, scrape.SeedersCount);
                written += Message.Write(buffer, written, scrape.CompleteCount);
                written += Message.Write(buffer, written, scrape.LeechesCount);
            }

            return written - offset;
        }

        #endregion Public Methods
    }
}

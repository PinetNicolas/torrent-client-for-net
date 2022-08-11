using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The announce response message.
    /// </summary>
    public class AnnounceResponseMessage : TrackerMessage
    {
        #region Private Fields

        /// <summary>
        /// The action length in bytes.
        /// </summary>
        private const int ActionLength = 4;

        /// <summary>
        /// The interval length in bytes.
        /// </summary>
        private const int IntervalLength = 4;

        /// <summary>
        /// The IP address length in bytes.
        /// </summary>
        private const int IpAddressLength = 4;

        /// <summary>
        /// The leechers length in bytes.
        /// </summary>
        private const int LeechersLength = 4;

        /// <summary>
        /// The port length in bytes.
        /// </summary>
        private const int PortLength = 2;

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
        /// Initializes a new instance of the <see cref="AnnounceResponseMessage" /> class.
        /// </summary>
        /// <param name="transactionId">The transaction unique identifier.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="leecherCount">The leecher count.</param>
        /// <param name="seederCount">The seeder count.</param>
        /// <param name="peers">The peers.</param>
        public AnnounceResponseMessage(int transactionId, TimeSpan interval, int leecherCount, int seederCount, IEnumerable<IPEndPoint> peers)
            : base(TrackingAction.Announce, transactionId)
        {
            this.Interval = interval;
            this.LeecherCount = leecherCount;
            this.SeederCount = seederCount;
            this.Peers = peers;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public TimeSpan Interval
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the leecher count.
        /// </summary>
        /// <value>
        /// The leecher count.
        /// </value>
        public int LeecherCount
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
                return ActionLength + TransactionIdLength + IntervalLength + LeechersLength + SeedersLength + (this.Peers.Count() * (IpAddressLength + PortLength));
            }
        }

        /// <summary>
        /// Gets the peer id / peer endpoint dictionary.
        /// </summary>
        /// <value>
        /// The peers.
        /// </value>
        public IEnumerable<IPEndPoint> Peers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the seeder count.
        /// </summary>
        /// <value>
        /// The seeder count.
        /// </value>
        public int SeederCount
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
        public static AnnounceResponseMessage TryDecode(byte[] buffer, int offset)
        {
            int action;
            int transactionId;
            int interval;
            int leechers;
            int seeders;
            IPEndPoint endpoint;
            IDictionary<string, IPEndPoint> peers = new Dictionary<string, IPEndPoint>();

            AnnounceResponseMessage message = null;

            if (buffer != null &&
                buffer.Length >= offset + ActionLength + TransactionIdLength + IntervalLength + LeechersLength + SeedersLength &&
                offset >= 0)
            {
                action = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                transactionId = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                interval = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                leechers = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                seeders = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;

                if (action == (int)TrackingAction.Announce &&
                    transactionId >= 0 &&
                    interval > 0 &&
                    leechers >= 0 &&
                    seeders >= 0)
                {
                    while (offset <= buffer.Length - IpAddressLength - PortLength)
                    {
                        endpoint = Message.ReadEndpoint(buffer, offset);
                        offset += Message.IntLength + Message.ShortLength;

                        if (!peers.ContainsKey(endpoint.Address.ToString()))
                        {
                            peers.Add(endpoint.Address.ToString(), endpoint);
                        }
                    }

                    message = new AnnounceResponseMessage(transactionId, TimeSpan.FromSeconds(interval), leechers, seeders, peers.Values);
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
            written += Message.Write(buffer, written, (int)this.Interval.TotalSeconds);
            written += Message.Write(buffer, written, this.LeecherCount);
            written += Message.Write(buffer, written, this.SeederCount);

            foreach (IPEndPoint peerEndpoint in this.Peers)
            {
                written += Message.Write(buffer, written, peerEndpoint);
            }

            return written - offset;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "UdpTrackerAnnounceResponseMessage";
        }

        #endregion Public Methods
    }
}

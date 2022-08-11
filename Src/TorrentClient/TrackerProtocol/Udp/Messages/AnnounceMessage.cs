using System;
using System.Net;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The announce message.
    /// </summary>
    public class AnnounceMessage : TrackerMessage
    {
        #region Private Fields

        /// <summary>
        /// The action length in bytes.
        /// </summary>
        private const int ActionLength = 4;

        /// <summary>
        /// The connection identifier length.
        /// </summary>
        private const int ConnectionIdLength = 8;

        /// <summary>
        /// The downloaded length in bytes.
        /// </summary>
        private const int DownloadedLength = 8;

        /// <summary>
        /// The information hash length in bytes.
        /// </summary>
        private const int InfoHashLength = 20;

        /// <summary>
        /// The IP address length in bytes.
        /// </summary>
        private const int IpAddressLength = 4;

        /// <summary>
        /// The key length in bytes.
        /// </summary>
        private const int KeyLength = 4;

        /// <summary>
        /// The left length in bytes.
        /// </summary>
        private const int LeftLength = 8;

        /// <summary>
        /// The number want length in bytes.
        /// </summary>
        private const int NumWantLength = 4;

        /// <summary>
        /// The peer identifier length in bytes.
        /// </summary>
        private const int PeerIdLength = 20;

        /// <summary>
        /// The port length in bytes.
        /// </summary>
        private const int PortLength = 2;

        /// <summary>
        /// The tracking event length in bytes.
        /// </summary>
        private const int TrackingEventLength = 4;

        /// <summary>
        /// The transaction identifier length in bytes.
        /// </summary>
        private const int TransactionIdLength = 4;

        /// <summary>
        /// The uploaded length in bytes.
        /// </summary>
        private const int UploadedLength = 8;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnounceMessage" /> class.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="transactionId">The transaction identifier.</param>
        /// <param name="infoHash">The information hash.</param>
        /// <param name="peerId">The peer identifier.</param>
        /// <param name="downloaded">The downloaded.</param>
        /// <param name="left">The left.</param>
        /// <param name="uploaded">The uploaded.</param>
        /// <param name="trackingEvent">The tracking event.</param>
        /// <param name="key">The key.</param>
        /// <param name="numberWanted">The number wanted.</param>
        /// <param name="endpoint">The endpoint.</param>
        public AnnounceMessage(long connectionId, int transactionId, string infoHash, string peerId, long downloaded, long left, long uploaded, TrackingEvent trackingEvent, uint key, int numberWanted, IPEndPoint endpoint)
            : base(TrackingAction.Announce, transactionId)
        {
            this.ConnectionId = connectionId;
            this.InfoHash = infoHash;
            this.PeerId = peerId;
            this.Downloaded = downloaded;
            this.Left = left;
            this.Uploaded = uploaded;
            this.TrackingEvent = trackingEvent;
            this.Key = key;
            this.NumberWanted = numberWanted;
            this.Endpoint = endpoint;
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
        /// Gets the downloaded byte count.
        /// </summary>
        /// <value>
        /// The downloaded byte count.
        /// </value>
        public long Downloaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public IPEndPoint Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the info hash.
        /// </summary>
        /// <value>
        /// The info hash.
        /// </value>
        public string InfoHash
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public uint Key
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the left byte count.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public long Left
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
                return ConnectionIdLength + ActionLength + TransactionIdLength + InfoHashLength + PeerIdLength + DownloadedLength + LeftLength + UploadedLength + TrackingEventLength + IpAddressLength + KeyLength + NumWantLength + PortLength;
            }
        }

        /// <summary>
        /// Gets the number of wanted peers.
        /// </summary>
        /// <value>
        /// The number of wanted peers.
        /// </value>
        public int NumberWanted
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the peer unique identifier.
        /// </summary>
        /// <value>
        /// The peer unique identifier.
        /// </value>
        public string PeerId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tracking event.
        /// </summary>
        /// <value>
        /// The tracking event.
        /// </value>
        public TrackingEvent TrackingEvent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the uploaded byte count.
        /// </summary>
        /// <value>
        /// The uploaded byte count.
        /// </value>
        public long Uploaded
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
        public static AnnounceMessage TryDecode(byte[] buffer, int offset)
        {
            long connectionId;
            long action;
            int transactionId;
            string infoHash;
            string peerId;
            long downloaded;
            long left;
            long uploaded;
            int trackingEvent;
            int ipaddress;
            uint key;
            int numberWanted;
            ushort port;

            AnnounceMessage message = null;

            if (buffer != null &&
                buffer.Length >= offset + ConnectionIdLength + ActionLength + TransactionIdLength + InfoHashLength + PeerIdLength + DownloadedLength + LeftLength + UploadedLength + TrackingEventLength + IpAddressLength + KeyLength + NumWantLength + PortLength &&
                offset >= 0)
            {
                connectionId = Message.ReadLong(buffer, offset);
                offset += Message.LongLength;
                action = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                transactionId = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                infoHash = Message.ReadBytes(buffer, offset, 20).ToHexaDecimalString();
                offset += 20;
                peerId = Message.ToPeerId(Message.ReadBytes(buffer, offset, 20));
                offset += 20;
                downloaded = Message.ReadLong(buffer, offset);
                offset += Message.LongLength;
                left = Message.ReadLong(buffer, offset);
                offset += Message.LongLength;
                uploaded = Message.ReadLong(buffer, offset);
                offset += Message.LongLength;
                trackingEvent = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                ipaddress = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                key = (uint)Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                numberWanted = Message.ReadInt(buffer, offset);
                offset += Message.IntLength;
                port = (ushort)Message.ReadShort(buffer, offset);
                offset += Message.ShortLength;

                if (connectionId >= 0 &&
                    action == (int)TrackingAction.Announce &&
                    transactionId >= 0 &&
                    infoHash != null && infoHash.Length > 0 &&
                    !string.IsNullOrEmpty(peerId) &&
                    downloaded >= 0 &&
                    left >= 0 &&
                    uploaded >= 0 &&
                    trackingEvent >= 0 &&
                    trackingEvent <= 3 &&
                    port >= IPEndPoint.MinPort &&
                    port <= IPEndPoint.MaxPort)
                {
                    message = new AnnounceMessage(connectionId, transactionId, infoHash, peerId, downloaded, left, uploaded, (TrackingEvent)trackingEvent, key, numberWanted, new IPEndPoint(new IPAddress(BitConverter.GetBytes(ipaddress)), port));
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
            written += Message.Write(buffer, written, this.InfoHash.ToByteArray());
            written += Message.Write(buffer, written, Message.FromPeerId(this.PeerId));
            written += Message.Write(buffer, written, this.Downloaded);
            written += Message.Write(buffer, written, this.Left);
            written += Message.Write(buffer, written, this.Uploaded);
            written += Message.Write(buffer, written, (int)this.TrackingEvent);
            written += Message.Write(buffer, written, this.Endpoint.Address == IPAddress.Loopback ? 0 : BitConverter.ToInt32(this.Endpoint.Address.GetAddressBytes(), 0));
            written += Message.Write(buffer, written, this.Key);
            written += Message.Write(buffer, written, this.NumberWanted);
            written += Message.Write(buffer, written, (ushort)this.Endpoint.Port);

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
            return "UdpTrackerAnnounceMessage";
        }

        #endregion Public Methods
    }
}

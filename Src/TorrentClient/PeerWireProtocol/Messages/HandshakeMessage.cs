﻿using System;
using System.Globalization;
using System.Text;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// The handshake message.
    /// </summary>
    public class HandshakeMessage : PeerMessage
    {
        #region Public Fields

        /// <summary>
        /// The protocol name.
        /// </summary>
        public const string ProtocolName = "BitTorrent protocol";

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The extended messaging flag.
        /// </summary>
        private const byte ExtendedMessagingFlag = 0x10;

        /// <summary>
        /// The fast peers flag.
        /// </summary>
        private const byte FastPeersFlag = 0x04;

        /// <summary>
        /// The information hash length in bytes.
        /// </summary>
        private const int InfoHashLength = 20;

        /// <summary>
        /// The name length length in bytes.
        /// </summary>
        private const int NameLengthLength = 1;

        /// <summary>
        /// The peer identifier length in bytes.
        /// </summary>
        private const int PeerIdLength = 20;

        /// <summary>
        /// The reserved length in bytes.
        /// </summary>
        private const int ReservedLength = 8;

        /// <summary>
        /// The zeroed bits.
        /// </summary>
        private static readonly byte[] ZeroedBits = new byte[8];

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HandshakeMessage" /> class.
        /// </summary>
        /// <param name="infoHash">The information hash.</param>
        /// <param name="peerId">The peer unique identifier.</param>
        /// <param name="protocolString">The protocol string.</param>
        /// <param name="supportsFastPeer">if set to <c>true</c> the peer supports fast peer.</param>
        /// <param name="supportsExtendedMessaging">if set to <c>true</c> the peer supports extended messaging.</param>
        /// <exception cref="TorrentClient.Exceptions.PeerWireProtocolException">The engine does not support fast peer, but fast peer was requested
        /// or
        /// The engine does not support extended, but extended was requested</exception>
        public HandshakeMessage(string infoHash, string peerId, string protocolString = ProtocolName, bool supportsFastPeer = false, bool supportsExtendedMessaging = false)
        {
            this.InfoHash = infoHash;
            this.PeerId = peerId;
            this.ProtocolString = protocolString;
            if(!string.IsNullOrEmpty(protocolString))
                this.ProtocolStringLength = protocolString.Length;
            this.SupportsFastPeer = supportsFastPeer;
            this.SupportsExtendedMessaging = supportsExtendedMessaging;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="HandshakeMessage"/> class from being created.
        /// </summary>
        private HandshakeMessage()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the information hash.
        /// </summary>
        /// <value>
        /// The information hash.
        /// </value>
        public string InfoHash
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
                return NameLengthLength + this.ProtocolString.Length + ReservedLength + InfoHashLength + PeerIdLength;
            }
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
        /// Gets the protocol string.
        /// </summary>
        /// <value>
        /// The protocol string.
        /// </value>
        public string ProtocolString
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the length of the protocol string.
        /// </summary>
        /// <value>
        /// The length of the protocol string.
        /// </value>
        public int ProtocolStringLength
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the peer supports extended messaging.
        /// </summary>
        /// <value>
        /// <c>true</c> if the peer supports extended messaging; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsExtendedMessaging
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the peer supports fast peer.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the peer supports fast peer; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsFastPeer
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
        public static HandshakeMessage TryDecode(byte[] buffer, int offsetFrom, int offsetTo)
        {
            byte protocolStringLength;
            string protocolString;
            bool supportsExtendedMessaging;
            bool supportsFastPeer;
            string infoHash;
            string peerId;
            int offsetStart = offsetFrom;
            HandshakeMessage message = new HandshakeMessage();

            if (buffer != null &&
                buffer.Length > offsetFrom + NameLengthLength + ReservedLength + InfoHashLength + PeerIdLength &&
                offsetFrom >= 0 &&
                offsetTo >= offsetFrom &&
                offsetTo <= buffer.Length)
            {
                protocolStringLength = Message.ReadByte(buffer, offsetStart); // first byte is length
                offsetStart++;
                if (buffer.Length >= offsetStart + protocolStringLength + ReservedLength + InfoHashLength + PeerIdLength)
                {
                    protocolString = Message.ReadString(buffer, offsetStart, protocolStringLength);
                    offsetStart += protocolStringLength;
                    // increment offset first so that the indices are consistent between Encoding and Decoding
                    offsetStart += ReservedLength;

                    supportsExtendedMessaging = (ExtendedMessagingFlag & buffer[offsetStart - 3]) == ExtendedMessagingFlag;
                    supportsFastPeer = (FastPeersFlag & buffer[offsetStart - 1]) == FastPeersFlag;

                    infoHash = Message.ReadBytes(buffer, offsetStart, 20).ToHexaDecimalString();
                    offsetStart += 20;
                    peerId = Message.ToPeerId(Message.ReadBytes(buffer, offsetStart, 20));
                    offsetStart += 20;

                    if (protocolStringLength == 19 &&
                        protocolString == ProtocolName &&
                        infoHash.Length == 40 &&
                        peerId != null &&
                        peerId.Length >= 20 &&
                        !string.IsNullOrEmpty(peerId))
                    {
                        if (offsetStart <= offsetTo)
                        {
                            message = new HandshakeMessage(infoHash, peerId, protocolString, supportsFastPeer, supportsExtendedMessaging);
                        }
                        else
                        {
                            message.IsIncomplete = true;
                        }
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

            written += Message.Write(buffer, written, (byte)this.ProtocolString.Length);
            written += Message.Write(buffer, written, this.ProtocolString);
            written += Message.Write(buffer, written, ZeroedBits);

            if (this.SupportsExtendedMessaging)
            {
                buffer[written - 3] |= ExtendedMessagingFlag;
            }

            if (this.SupportsFastPeer)
            {
                buffer[written - 1] |= FastPeersFlag;
            }

            written += Message.Write(buffer, written, this.InfoHash.ToByteArray());
            written += Message.Write(buffer, written, Message.FromPeerId(this.PeerId));

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
            HandshakeMessage msg = obj as HandshakeMessage;

            if (msg == null)
            {
                return false;
            }
            else
            {
                if (this.InfoHash != msg.InfoHash)
                {
                    return false;
                }
                else
                {
                    return this.InfoHash == msg.InfoHash &&
                           this.PeerId == msg.PeerId &&
                           this.ProtocolString == msg.ProtocolString &&
                           this.SupportsFastPeer == msg.SupportsFastPeer &&
                           this.SupportsExtendedMessaging == msg.SupportsExtendedMessaging;
                }
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
            return this.InfoHash.GetHashCode(StringComparison.InvariantCulture) ^
                   this.PeerId.GetHashCode(StringComparison.InvariantCulture) ^
                   this.ProtocolString.GetHashCode(StringComparison.InvariantCulture) ^
                   this.SupportsFastPeer.GetHashCode() ^
                   this.SupportsExtendedMessaging.GetHashCode();
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
            sb.Append("HandshakeMessage: ");
            sb.AppendFormat(CultureInfo.InvariantCulture,"PeerID = {0}",this.PeerId);
            sb.AppendFormat(CultureInfo.InvariantCulture, "InfoHash = {0}", this.InfoHash);
            sb.AppendFormat(CultureInfo.InvariantCulture, "FastPeer = {0}", this.SupportsFastPeer);
            sb.AppendFormat(CultureInfo.InvariantCulture, "ExtendedMessaging = {0}", this.SupportsExtendedMessaging);

            return sb.ToString();
        }

        #endregion Public Methods
    }
}

using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The tracker message base.
    /// </summary>
    public abstract class TrackerMessage : Message
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerMessage"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="transactionId">The transaction unique identifier.</param>
        public TrackerMessage(TrackingAction action, int transactionId)
        {
            this.Action = action;
            this.TransactionId = transactionId;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public TrackingAction Action
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the transaction unique identifier.
        /// </summary>
        /// <value>
        /// The transaction unique identifier.
        /// </value>
        public int TransactionId
        {
            get;
            protected set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Tries to decode the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        /// The message decode or null if problem
        /// </returns>
        public static TrackerMessage TryDecode(byte[] buffer, int offset, MessageType messageType)
        {
            int action;

            TrackerMessage message = null;

            if (buffer!=null && buffer.Length > 0)
            {
                action = messageType == MessageType.Request ? Message.ReadInt(buffer, offset) : Message.ReadInt(buffer, offset);
                offset = 0;

                if (action == (int)TrackingAction.Connect)
                {
                    if (messageType == MessageType.Request)
                    {
                        message = ConnectMessage.TryDecode(buffer, offset);
                    }
                    else
                    {
                        message = ConnectResponseMessage.TryDecode(buffer, offset);
                    }
                }
                else if (action == (int)TrackingAction.Announce)
                {
                    if (messageType == MessageType.Request)
                    {
                        message = AnnounceMessage.TryDecode(buffer, offset);
                    }
                    else
                    {
                        message = AnnounceResponseMessage.TryDecode(buffer, offset);
                    }
                }
                else if (action == (int)TrackingAction.Scrape)
                {
                    if (messageType == MessageType.Request)
                    {
                        message = ScrapeMessage.TryDecode(buffer, offset);
                    }
                    else
                    {
                        message = ScrapeResponseMessage.TryDecode(buffer, offset);
                    }
                }
                else if (action == (int)TrackingAction.Error)
                {
                    message = ErrorMessage.TryDecode(buffer, offset);
                }
                else
                {
                    // could not decode UDP message
                }
            }

            return message;
        }

        #endregion Public Methods
    }
}

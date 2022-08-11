using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The interested message test.
    /// </summary>
    [TestClass]
    public class InterestedMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void InterestedMessage_TryDecode()
        {
            int offsetFrom = 0;
            byte[] data = "0000000102".ToByteArray();

            InterestedMessage message = InterestedMessage.TryDecode(data, offsetFrom, data.Length);
                Assert.AreEqual(5, message.Length);
                Assert.AreEqual(false, message.IsIncomplete);
                CollectionAssert.AreEqual(data, message.Encode());
        }

        #endregion Public Methods
    }
}
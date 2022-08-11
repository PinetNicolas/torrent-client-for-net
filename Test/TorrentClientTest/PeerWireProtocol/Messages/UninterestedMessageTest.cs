using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The interested message test.
    /// </summary>
    [TestClass]
    public class UninterestedMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void UninterestedMessage_TryDecode()
        {
            int offset = 0;
            byte[] data = "0000000103".ToByteArray();

            UninterestedMessage message = UninterestedMessage.TryDecode(data, offset, data.Length);
            if (message != null)
            {
                Assert.AreEqual(5, message.Length);
                Assert.AreEqual(false, message.IsIncomplete);
                Assert.AreEqual(data.Length, message.Length);
                CollectionAssert.AreEqual(data, message.Encode());
            }
            else
            {
                Assert.Fail();
            }
        }

        #endregion Public Methods
    }
}
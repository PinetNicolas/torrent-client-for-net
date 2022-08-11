using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The request message test.
    /// </summary>
    [TestClass]
    public class RequestMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void RequestMessage_TryDecode()
        {
            int offset = 0;
            bool isIncomplete;
            byte[] data = "0000000D06000000050000000600000007".ToByteArray();

            RequestMessage message = RequestMessage.TryDecode(data, offset, data.Length);
            if (message != null)
            {
                Assert.AreEqual(17, message.Length);
                Assert.AreEqual(5, message.PieceIndex);
                Assert.AreEqual(6, message.BlockOffset);
                Assert.AreEqual(7, message.BlockLength);
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The piece message test.
    /// </summary>
    [TestClass]
    public class PieceMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void TestTryDecodePieceMessge()
        {
            int offset = 0;
            byte[] data = "0000000B070000000500000006ABCD".ToByteArray();
            bool isIncomplete;

            PieceMessage message = PieceMessage.TryDecode(data, offset, data.Length);
            if (message != null)
            {
                Assert.AreEqual(15, message.Length);
                Assert.AreEqual(5, message.PieceIndex);
                Assert.AreEqual(6, message.BlockOffset);
                Assert.AreEqual(171, message.Data[0]);
                Assert.AreEqual(205, message.Data[1]);
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
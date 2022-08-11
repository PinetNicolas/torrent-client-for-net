using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The have message test.
    /// </summary>
    [TestClass]
    public class HaveMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void HaveMessage_TryDecode()
        {
            int offset = 0;
            bool isIncomplete;
            byte[] data = "0000000504000000AA".ToByteArray();

            HaveMessage message = HaveMessage.TryDecode(data, offset, data.Length);
            if (message != null)
            {
                Assert.AreEqual(9, message.Length);
                Assert.AreEqual(170, message.PieceIndex);
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
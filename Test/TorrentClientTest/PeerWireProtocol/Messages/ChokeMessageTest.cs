using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The choke message test.
    /// </summary>
    [TestClass]
    public class ChokeMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void ChokeMessage_TryDecode()
        {
            int offset = 0;
            bool isIncomplete;
            byte[] data = "0000000100".ToByteArray();

            ChokeMessage message = ChokeMessage.TryDecode(data, offset, data.Length);
            if (message !=null)
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
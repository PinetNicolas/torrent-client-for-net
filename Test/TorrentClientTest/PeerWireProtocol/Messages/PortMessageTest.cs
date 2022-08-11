using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The port message test.
    /// </summary>
    [TestClass]
    public class PortMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void PortMessage_TryDecode()
        {
            int offset = 0;
            byte[] data = "00000003090FAC".ToByteArray();

            PortMessage message = PortMessage.TryDecode(data, offset, data.Length);
            if (message != null)
            {
                Assert.AreEqual(7, message.Length);
                Assert.AreEqual(4012, message.Port);
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
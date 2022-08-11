using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.TrackerProtocol.Udp.Messages;

namespace TorrentClient.Test.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The connect message test.
    /// </summary>
    [TestClass]
    public class ConnectMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the try decode message.
        /// </summary>
        [TestMethod]
        public void ConnectMessage_TryDecode()
        {
            byte[] data = "00000417271019800000000000003300".ToByteArray();

            ConnectMessage message = ConnectMessage.TryDecode(data, 0);
            if (message != null)
            {
                Assert.AreEqual(16, message.Length);
                Assert.AreEqual(0, (int)message.Action);
                Assert.AreEqual(13056, message.TransactionId);
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
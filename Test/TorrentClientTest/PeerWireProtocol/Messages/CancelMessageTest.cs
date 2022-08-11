﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The cancel message test.
    /// </summary>
    [TestClass]
    public class CancelMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void CancelMessage_TryDecode()
        {
            int offset = 0;
            bool isIncomplete;
            byte[] data = "0000000D08000000050000000600000007".ToByteArray();

            CancelMessage message = CancelMessage.TryDecode(data, offset, data.Length);
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
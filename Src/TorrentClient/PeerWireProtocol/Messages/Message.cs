using System;
using System.Data;
using System.Net;
using System.Text;
using TorrentClient.Exceptions;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// The message base class.
    /// </summary>
    public abstract class Message
    {
        #region Public Fields

        /// <summary>
        /// The byte length in bytes.
        /// </summary>
        public const int ByteLength = 1;

        /// <summary>
        /// The integer length in bytes.
        /// </summary>
        public const int IntLength = 4;

        /// <summary>
        /// The long length in bytes.
        /// </summary>
        public const int LongLength = 8;

        /// <summary>
        /// The short length in bytes.
        /// </summary>
        public const int ShortLength = 2;

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets the length in bytes.
        /// </summary>
        /// <value>
        /// The length in bytes.
        /// </value>
        public abstract int Length
        {
            get;
        }

        public bool IsIncomplete { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Copies the bytes from the specified source array to the specified destination array.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="sourceOffset">The source offset.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationOffset">The destination offset.</param>
        /// <param name="count">The count.</param>
        public static void Copy(byte[] source, int sourceOffset, byte[] destination, int destinationOffset, int count)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (destinationOffset < 0 || destinationOffset > destination.Length) throw new ArgumentOutOfRangeException("destinationOffset");

            if (source == null) throw new ArgumentNullException("source");
            if (sourceOffset < 0 || sourceOffset > source.Length) throw new ArgumentOutOfRangeException("sourceOffset");

            if (count > destination.Length - destinationOffset
                    || count > source.Length - sourceOffset) throw new ArgumentOutOfRangeException("count");

            Buffer.BlockCopy(source, sourceOffset, destination, destinationOffset, count);
        }

        /// <summary>
        /// Converts peer id to binary array.
        /// </summary>
        /// <param name="peerId">The peer identifier.</param>
        /// <returns>The peer id binary array.</returns>
        public static byte[] FromPeerId(string peerId)
        {
            if(!string.IsNullOrEmpty(peerId) && !peerId.Contains("-", StringComparison.InvariantCulture))
                throw new ArgumentException("peerId must contains -", "peerId");

            int delimiterIndex;

            delimiterIndex = peerId.LastIndexOf('-');

            string newId = Encoding.ASCII.GetBytes(peerId.Substring(0, delimiterIndex + 1)).ToHexaDecimalString() + // client id
                     peerId.Substring(delimiterIndex + 1); // random number

            if (newId.Length != 40)
                throw new EvaluateException("newId length error");

            return newId.ToByteArray();
        }

        /// <summary>
        /// Reads the byte form the buffer at the specified offset by advancing the offset afterwards.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The read byte.</returns>
        public static byte ReadByte(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - ByteLength)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - ByteLength}");

            byte value;

            value = Buffer.GetByte(buffer, offset);

            return value;
        }

        /// <summary>
        /// Reads the byte form the buffer starting at the specified offset for the specified count by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// The array of read bytes
        /// </returns>
        public static byte[] ReadBytes(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - count)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - count}");
            if (count < 0 || count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("count", $"count must be between 0 and {buffer.Length - offset}");

            byte[] result = new byte[count];

            Buffer.BlockCopy(buffer, offset, result, 0, count);

            return result;
        }

        /// <summary>
        /// Reads the endpoint from the buffer starting at the specified offset by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The endpoint.</returns>
        public static IPEndPoint ReadEndpoint(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - IntLength - ShortLength)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - IntLength - ShortLength}");

            byte[] ipaddress = ReadBytes(buffer, offset, IntLength);
            offset += IntLength;
            int port = (ushort)ReadShort(buffer, offset);

            return new IPEndPoint(new IPAddress(ipaddress), port);
        }

        /// <summary>
        /// Reads the integer form the buffer starting at the specified offset by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The integer.</returns>
        public static int ReadInt(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - IntLength)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - IntLength}");

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, offset));
        }

        /// <summary>
        /// Reads the long number form the buffer starting at the specified offset by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The long number.</returns>
        public static long ReadLong(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - LongLength)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - LongLength}");
            
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buffer, offset));
        }

        /// <summary>
        /// Reads the short number form the buffer starting at the specified offset by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The short number.</returns>
        public static short ReadShort(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - ShortLength)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length -ShortLength}");

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, offset));
        }

        /// <summary>
        /// Reads the string form the buffer starting at the specified offset for the specified count by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// The string.
        /// </returns>
        public static string ReadString(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - count)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - count}");
            if (count < 0 || count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("count", $"Count must be between 0 and {buffer.Length - offset}");

            return Encoding.ASCII.GetString(buffer, offset, count);
        }

        /// <summary>
        /// Reads the peer identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The peer identifier.
        /// </returns>
        public static string ToPeerId(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "value can't be null");
            if (value.Length != 20)
                throw new ArgumentException("value must be 20","value");

            int delimiterIndex = -1;
            int offset = 0;
            string peerId = null;

            for (int i = 0; i < value.Length; i++)
            {
                if ((char)value[i] == '-')
                {
                    delimiterIndex = i;
                }
            }

            if (delimiterIndex > 0)
            {
                peerId = ReadString(value, offset, delimiterIndex + 1); // client id
                offset += delimiterIndex + 1;
                peerId += ReadBytes(value, offset, value.Length - delimiterIndex - 1).ToHexaDecimalString(); // random number
                offset += value.Length - delimiterIndex - 1;
            }
            else
            {
                peerId = ReadBytes(value, offset, value.Length).ToHexaDecimalString(); // could not interpret peer id -> read as binary string
                offset += value.Length;
            }

            return peerId;
        }

        /// <summary>
        /// Writes the specified byte value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of byte write</returns>
        public static int Write(byte[] buffer, int offset, byte value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length}");

            Buffer.SetByte(buffer, offset, value);
            return 1;
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of byte write</returns>
        public static int Write(byte[] buffer, int offset, ushort value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - ShortLength)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - ShortLength}");

            Write(buffer, offset, (short)value);
            return ShortLength;
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of byte write</returns>
        public static int Write(byte[] buffer, int offset, short value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - ShortLength)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - ShortLength}");

            Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)), 0, buffer, offset, ShortLength);
            return ShortLength;
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of byte write</returns>
        public static int Write(byte[] buffer, int offset, IPAddress value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length}");

            if (value == null)
                throw new ArgumentNullException("value", "value can't be null");

            return Write(buffer, offset, value.GetAddressBytes());
             
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of byte write</returns>
        public static int Write(byte[] buffer, int offset, IPEndPoint value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length}");

            if (value == null)
                throw new ArgumentNullException("value", "value can't be null");

           int wrt = Write(buffer, offset, value.Address);
            offset += wrt;
            return wrt + Write(buffer, offset, (ushort)value.Port);
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of byte write</returns>
        public static int Write(byte[] buffer, int offset, int value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - IntLength)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - IntLength}");

            Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)), 0, buffer, offset, IntLength);
            return IntLength;
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of byte write</returns>
        public static int Write(byte[] buffer, int offset, uint value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - IntLength)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - IntLength}");

            Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)value)), 0, buffer, offset, IntLength);
            return IntLength;
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of byte write</returns>
        public static int Write(byte[] buffer, int offset, long value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - LongLength)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - LongLength}");
            
            Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)), 0, buffer, offset, LongLength);
            return LongLength;
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of byte write</returns>
        public static int Write(byte[] buffer, int offset, byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "value can't be null");
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length - value.Length)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length - value.Length}");
            
            Copy(value, 0, buffer, offset, value.Length);
            return value.Length;
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of byte write</returns>
        public static int Write(byte[] buffer, int offset, string value)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "buffer can't be null");
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset", $"Offset must be between 0 and {buffer.Length}");

            byte[] data;

            data = Encoding.ASCII.GetBytes(value);

            Copy(data, 0, buffer, offset, data.Length);
            return data.Length;
        }

        /// <summary>
        /// Encodes the message.
        /// </summary>
        /// <returns>The byte array.</returns>
        public byte[] Encode()
        {
            byte[] buffer = new byte[this.Length];

            this.Encode(buffer, 0);

            return buffer;
        }

        /// <summary>
        /// Encodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The number of encoded bytes.</returns>
        public abstract int Encode(byte[] buffer, int offset);

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Checks the written.
        /// </summary>
        /// <param name="written">The written.</param>
        /// <returns>The written byte count.</returns>
        protected int CheckWritten(int written)
        {
            if (written != this.Length)
            {
                throw new MessageException("Message encoded incorrectly. Incorrect number of bytes written");
            }
            else
            {
                return written;
            }
        }

        #endregion Protected Methods
    }
}

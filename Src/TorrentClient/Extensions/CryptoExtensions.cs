﻿using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TorrentClient.Extensions
{
    /// <summary>
    /// The cryptography extensions.
    /// </summary>
    public static class CryptoExtensions
    {
        #region Public Methods

        /// <summary>
        /// Calculates the 128 bit SHA hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>The 128 bit SHA hash.</returns>
        public static byte[] CalculateSha1Hash(this byte[] data, int offset, int length)
        {
            return CalculateHashSha(data, offset, length, 128);
        }

        /// <summary>
        /// Calculates the 128 bit SHA hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>
        /// The 128 bit SHA hash.
        /// </returns>
        public static byte[] CalculateSha1Hash(this byte[] data)
        {
            if (data == null)  throw new ArgumentNullException("data");
            return CalculateHashSha(data, 0, data.Length, 128);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Calculates the hash of a string.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <returns>
        /// Calculated hash
        /// </returns>
        private static byte[] CalculateHash(this byte[] data, int offset, int count, HashAlgorithm hashAlgorithm)
        {
            byte[] hashRaw = hashAlgorithm.ComputeHash(data, offset, count);
            hashAlgorithm.Clear();

            return hashRaw;
        }

        /// <summary>
        /// Calculates the hash of a data using SHA hash algorithm.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="hashSize">Size of the hash.</param>
        /// <returns>
        /// Calculated hash
        /// </returns>
        private static byte[] CalculateHashSha(this byte[] data, int offset, int length, int hashSize)
        {
            byte[] hashRaw = null;

            if (data != null)
            {
                if (hashSize == 128)
                {
                    using (HashAlgorithm sha = new SHA1CryptoServiceProvider())
                    {
                        hashRaw = CalculateHash(data, offset, length, sha);
                    }
                }
                else if (hashSize == 256)
                {
                    try
                    {
                        using (HashAlgorithm sha = new SHA256CryptoServiceProvider())
                        {
                            hashRaw = CalculateHash(data, offset, length, sha);
                        }
                    }
                    catch (PlatformNotSupportedException)
                    {
                        // Fall back to the managed version if the CSP
                        // is not supported on this platform.
                        using (HashAlgorithm sha = new SHA256Managed())
                        {
                            hashRaw = CalculateHash(data, offset, length, sha);
                        }
                    }
                }
                else if (hashSize == 384)
                {
                    try
                    {
                        using (HashAlgorithm sha = new SHA384CryptoServiceProvider())
                        {
                            hashRaw = CalculateHash(data, offset, length, sha);
                        }
                    }
                    catch (PlatformNotSupportedException)
                    {
                        // Fall back to the managed version if the CSP
                        // is not supported on this platform.
                        using (HashAlgorithm sha = new SHA384Managed())
                        {
                            hashRaw = CalculateHash(data, offset, length, sha);
                        }
                    }
                }
                else if (hashSize == 512)
                {
                    try
                    {
                        using (HashAlgorithm sha = new SHA512CryptoServiceProvider())
                        {
                            hashRaw = CalculateHash(data, offset, length, sha);
                        }
                    }
                    catch (PlatformNotSupportedException)
                    {
                        // Fall back to the managed version if the CSP
                        // is not supported on this platform.
                        using (HashAlgorithm sha = new SHA512Managed())
                        {
                            hashRaw = CalculateHash(data, offset, length, sha);
                        }
                    }
                }
            }

            return hashRaw;
        }

        #endregion Private Methods
    }
}

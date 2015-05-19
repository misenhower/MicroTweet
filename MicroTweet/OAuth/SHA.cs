using System;
using Microsoft.SPOT;

namespace MicroTweet
{
    internal static class SHA
    {
        /// <summary>
        /// Computes the SHA-1 hash value for the specified message.
        /// </summary>
        /// <param name="message">The message to compute the hash for.</param>
        // Adapted from: http://en.wikipedia.org/wiki/SHA-1#SHA-1_pseudocode
        public static byte[] ComputeSHA1(byte[] message)
        {
            // Initialize variables
            uint h0 = 0x67452301;
            uint h1 = 0xEFCDAB89;
            uint h2 = 0x98BADCFE;
            uint h3 = 0x10325476;
            uint h4 = 0xC3D2E1F0;
            uint a, b, c, d, e, f, k, temp;
            uint[] w = new uint[80];
            int i;

            // Pad the end of the message and add the length in bits (as a 64-bit integer)
            // First, determine the new (processed) message length: at minimum, we need to add one byte (0x80) followed by 8 bytes (for the 64-bit integer that represents the original message length)
            int processedMessageLength = message.Length + 1 + 8;

            // The total length must be a multiple of 512 bits (64 bytes)
            int remainder = processedMessageLength % 64;
            if (remainder > 0)
                processedMessageLength += (64 - remainder);

            // Copy the message to a new, larger array
            byte[] processedMessage = new byte[processedMessageLength];
            Array.Copy(message, processedMessage, message.Length);

            // Add the message padding byte
            processedMessage[message.Length] = 0x80;

            // Add the message length in bits (as a big-endian 64-bit integer)
            ulong messageLength = (ulong)message.Length * 8;
            byte[] lengthBytes = BitConverter.GetBytes(ReverseBytes(messageLength));
            Array.Copy(lengthBytes, 0, processedMessage, processedMessageLength - 8, 8);

            // Process the message in 512-bit (64-byte) chunks
            for (int index = 0; index < processedMessage.Length; index += 64)
            {
                // Break the chunk into sixteen 32-bit big-endian words
                for (i = 0; i < 16; i++)
                    w[i] = ReverseBytes(BitConverter.ToUInt32(processedMessage, index + i * 4));

                // Extend the sixteen 32-bit words into eighty 32-bit words
                for (i = 16; i < 80; i++)
                    w[i] = LeftRotate(w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16], 1);

                // Initialize hash value for this chunk
                a = h0;
                b = h1;
                c = h2;
                d = h3;
                e = h4;

                // Main loop
                for (i = 0; i < 80; i++)
                {
                    if (i <= 19)
                    {
                        f = (b & c) | ((~b) & d);
                        k = 0x5A827999;
                    }
                    else if (i <= 39)
                    {
                        f = b ^ c ^ d;
                        k = 0x6ED9EBA1;
                    }
                    else if (i <= 59)
                    {
                        f = (b & c) | (b & d) | (c & d);
                        k = 0x8F1BBCDC;
                    }
                    else
                    {
                        f = b ^ c ^ d;
                        k = 0xCA62C1D6;
                    }

                    temp = LeftRotate(a, 5) + f + e + k + w[i];
                    e = d;
                    d = c;
                    c = LeftRotate(b, 30);
                    b = a;
                    a = temp;
                }

                // Add this chunk's hash to result so far
                h0 += a;
                h1 += b;
                h2 += c;
                h3 += d;
                h4 += e;
            }

            // Final output
            byte[] result = new byte[20];
            Array.Copy(BitConverter.GetBytes(ReverseBytes(h0)), 0, result, 0, 4);
            Array.Copy(BitConverter.GetBytes(ReverseBytes(h1)), 0, result, 4, 4);
            Array.Copy(BitConverter.GetBytes(ReverseBytes(h2)), 0, result, 8, 4);
            Array.Copy(BitConverter.GetBytes(ReverseBytes(h3)), 0, result, 12, 4);
            Array.Copy(BitConverter.GetBytes(ReverseBytes(h4)), 0, result, 16, 4);

            return result;
        }

        /// <summary>
        /// Computes the HMAC-SHA1 value for the specified key and message.
        /// </summary>
        /// <param name="key">The secret cryptographic key. If the key is longer than 64 bytes it will be hashed (using SHA-1) to derive a 64-byte key.</param>
        /// <param name="message">The message to compute the hash for.</param>
        // Adapted from: http://en.wikipedia.org/wiki/Hash-based_message_authentication_code#Implementation
        public static byte[] ComputeHmacSHA1(byte[] key, byte[] message)
        {
            const int blocksize = 64;

            // Keys longer than blocksize are shortened
            if (key.Length > blocksize)
                key = ComputeSHA1(key);

            // Keys are zero-padded
            byte[] o_key_pad = new byte[blocksize + 20];
            byte[] i_key_pad = new byte[blocksize + message.Length];
            Array.Copy(key, o_key_pad, key.Length);
            Array.Copy(key, i_key_pad, key.Length);

            for (int i = 0; i < 64; i++)
            {
                o_key_pad[i] = (byte)(o_key_pad[i] ^ 0x5C);
                i_key_pad[i] = (byte)(i_key_pad[i] ^ 0x36);
            }

            // Re-use i_key_pad array to find intermediate hash
            Array.Copy(message, 0, i_key_pad, 64, message.Length);
            byte[] hash = ComputeSHA1(i_key_pad);

            // Re-use o_key_pad array to find final hash
            Array.Copy(hash, 0, o_key_pad, 64, 20);
            byte[] result = ComputeSHA1(o_key_pad);

            return result;
        }

        /// <summary>
        /// Converts a uint value between big-endian and little-endian representations.
        /// </summary>
        /// <param name="value">The uint value to convert.</param>
        private static uint ReverseBytes(uint value)
        {
            return
                (value & 0x000000FFu) << 24 |
                (value & 0x0000FF00u) << 8 |
                (value & 0x00FF0000u) >> 8 |
                (value & 0xFF000000u) >> 24;
        }

        /// <summary>
        /// Converts a ulong value between big-endian and little-endian representations.
        /// </summary>
        /// <param name="value">The ulong value to convert.</param>
        private static ulong ReverseBytes(ulong value)
        {
            return
                (value & 0x00000000000000FFul) << 56 |
                (value & 0x000000000000FF00ul) << 40 |
                (value & 0x0000000000FF0000ul) << 24 |
                (value & 0x00000000FF000000ul) << 8 |
                (value & 0x000000FF00000000ul) >> 8 |
                (value & 0x0000FF0000000000ul) >> 24 |
                (value & 0x00FF000000000000ul) >> 40 |
                (value & 0xFF00000000000000ul) >> 56;
        }

        /// <summary>
        /// Applies a left bitwise rotation (filling the right bit positions with the bits that were shifted out of the sequence).
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <param name="count">The number of bits to rotate the input value by.</param>
        /// <returns></returns>
        private static uint LeftRotate(uint input, int count)
        {
            return (input << count) | (input >> (32 - count));
        }
    }
}

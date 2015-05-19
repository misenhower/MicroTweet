using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;

namespace MicroTweet
{
    /// <summary>
    /// SNTP utility class included for convenience (since Twitter OAuth requests require a valid timestamp).
    /// </summary>
    public static class Sntp
    {
        /// <summary>
        /// Retrieves the current time from the specified SNTP server on the specified port.
        /// </summary>
        /// <param name="server">The hostname of the NTP server.</param>
        /// <param name="port">The UDP port the request will be sent to.</param>
        public static DateTime GetCurrentUtcTime(string server = "time-a.nist.gov", int port = 123)
        {
            // Resolve the hostname to an IP
            IPAddress ip = Dns.GetHostEntry(server).AddressList[0];

            // Set up the IP endpoint
            EndPoint remoteEndPoint = new IPEndPoint(ip, port);

            // Set up the send/receive buffer
            byte[] buffer = new byte[48];

            // Request byte 0:
            // Bits 0-2: Mode = 3 (Client)
            // Bits 3-5: Version = 3
            // Bits 6-7: Leap Indicator = 0 (None)
            buffer[0] = 0x1B;

            // Set up the socket
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.SendTo(buffer, remoteEndPoint);
                socket.ReceiveFrom(buffer, ref remoteEndPoint);
            }

            // Parse the response (starting at byte index 40)
            int index = 40;

            // Seconds since January 1, 1900
            ulong seconds = Utility.ReverseBytes(BitConverter.ToUInt32(buffer, index));
            index += 4;
            // Fractional seconds
            ulong milliseconds = Utility.ReverseBytes(BitConverter.ToUInt32(buffer, index));
            milliseconds = (milliseconds * 1000) / 0x100000000L;
            milliseconds += seconds * 1000;

            // Result
            return (new DateTime(1900, 1, 1)).AddMilliseconds(milliseconds);
        }
    }
}

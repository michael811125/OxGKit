using Cysharp.Threading.Tasks;
using OxGKit.LoggingSystem;
using System;
using System.Net;
using System.Net.Sockets;

namespace OxGKit.Utilities.Timer
{
    /**
     * Modified from: https://github.com/disas69/Unity-NTPTimeSync-Asset/blob/master/Assets/Scripts/NtpDateTime.cs
     */

    public static class NtpTime
    {
        private static DateTime _responseReceivedTime;
        private static DateTime _ntpDate;
        private static Socket _socket;
        private static bool _isSynchronized = false;

        /// <summary>
        /// Ntp date to universal time
        /// </summary>
        /// <returns></returns>
        public static DateTime GetUtcNow()
        {
            if (_isSynchronized)
                return _ntpDate.AddSeconds(DateTime.Now.Subtract(_responseReceivedTime).TotalSeconds).ToUniversalTime();
            Logging.Print<Logger>("<color=#ff8887>[NTP] No synchronized.</color>");
            return DateTime.Now.ToUniversalTime();
        }

        /// <summary>
        /// Ntp date to local time
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNow()
        {
            if (_isSynchronized)
                return _ntpDate.AddSeconds(DateTime.Now.Subtract(_responseReceivedTime).TotalSeconds).ToLocalTime();
            Logging.Print<Logger>("<color=#ff8887>[NTP] No synchronized.</color>");
            return DateTime.Now.ToLocalTime();
        }

        /// <summary>
        /// Ntp date
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNtpDate()
        {
            if (_isSynchronized)
                return _ntpDate.AddSeconds(DateTime.Now.Subtract(_responseReceivedTime).TotalSeconds);
            Logging.Print<Logger>("<color=#ff8887>[NTP] No synchronized.</color>");
            return DateTime.Now;
        }

        /// <summary>
        /// Check is synchronized with NTP server
        /// </summary>
        /// <returns></returns>
        public static bool IsSynchronized()
        {
            return _isSynchronized;
        }

        /// <summary>
        /// Begin synchronize with NTP server
        /// </summary>
        /// <param name="ntpServer"></param>
        /// <param name="requestTimeout"></param>
        /// <returns></returns>
        public static UniTask Synchronize(string ntpServer = "time.google.com", int requestTimeout = 3)
        {
            _isSynchronized = false;
            return _SynchronizeDate(ntpServer, requestTimeout);
        }

        private static async UniTask _SynchronizeDate(string ntpServer, int requestTimeout)
        {
            if (!_ConnectionEnabled())
            {
                Logging.Print<Logger>("<color=#ff8887>[NTP] Network not reachable.</color>");
                return;
            }

            await UniTask.SwitchToThreadPool();
            _BeginRequest(ntpServer, requestTimeout);
        }

        private static void _BeginRequest(string ntpServer, int requestTimeout)
        {
            Logging.Print<Logger>("<color=#f7ff87>[NTP] Request started.</color>");

            if (_socket != null)
                _socket.Close();

            var ntpData = new byte[48];
            ntpData[0] = 0x1B;

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                _socket.Connect(ipEndPoint);
                _socket.ReceiveTimeout = requestTimeout * 1000;
                _socket.Send(ntpData);
                _socket.Receive(ntpData);
            }
            catch (SocketException)
            {
                Logging.Print<Logger>("<color=#ff8887>[NTP] Sync failed.</color>");
                return;
            }
            finally
            {
                _socket.Close();
                _socket = null;
            }

            _ProcessResponse(ntpData);
        }

        private static void _ProcessResponse(byte[] ntpData)
        {
            Logging.Print<Logger>("<color=#f7ff87>[NTP] Response received.</color>");

            _responseReceivedTime = DateTime.Now;

            // Big-Endian
            var intPart = ((ulong)ntpData[40] << 24) | ((ulong)ntpData[41] << 16) | ((ulong)ntpData[42] << 8) | ntpData[43];
            var fractPart = ((ulong)ntpData[44] << 24) | ((ulong)ntpData[45] << 16) | ((ulong)ntpData[46] << 8) | ntpData[47];

            var milliseconds = intPart * 1000 + fractPart * 1000 / 0x100000000L;
            _ntpDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)milliseconds);

            _isSynchronized = true;

            Logging.Print<Logger>("<color=#f7ff87>[NTP] Date is synchronized.</color>");
        }

        private static bool _ConnectionEnabled()
        {
            return UnityEngine.Application.internetReachability != UnityEngine.NetworkReachability.NotReachable;
        }
    }
}
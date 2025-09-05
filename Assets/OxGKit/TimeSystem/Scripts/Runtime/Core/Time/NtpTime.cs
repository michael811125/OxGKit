using Cysharp.Threading.Tasks;
using OxGKit.LoggingSystem;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace OxGKit.TimeSystem
{
    /**
     * Modified from: https://github.com/disas69/Unity-NTPTimeSync-Asset/blob/master/Assets/Scripts/NtpDateTime.cs
     */

    [Serializable]
    public abstract class TimeApiResponseFormat
    {
    }

    /// <summary>
    /// Fields from TimeAPI.io response (Timezone endpoint)
    /// </summary>
    [Serializable]
    public class TimeApiTimezoneResponse : TimeApiResponseFormat
    {
        /// <summary>
        /// "Asia/Taipei"
        /// </summary>
        public string timeZone;

        /// <summary>
        /// "2025-05-15T12:55:08.5386566"
        /// </summary>
        public string currentLocalTime;

        public UtcOffset currentUtcOffset;
        public UtcOffset standardUtcOffset;
        public bool hasDayLightSaving;
        public bool isDayLightSavingActive;
        public DstInterval dstInterval;
    }

    [Serializable]
    public class DstInterval
    {
        public string dstName;
        public UtcOffset dstOffsetToUtc;
        public UtcOffset dstOffsetToStandardTime;
        public string dstStart;
        public string dstEnd;
        public DstDuration dstDuration;
    }

    [Serializable]
    public class DstDuration
    {
        public long days;
        public long nanosecondOfDay;
        public long hours;
        public long minutes;
        public long seconds;
        public long milliseconds;
        public long subsecondTicks;
        public long subsecondNanoseconds;
        public long bclCompatibleTicks;
        public double totalDays;
        public double totalHours;
        public double totalMinutes;
        public double totalSeconds;
        public double totalMilliseconds;
        public long totalTicks;
        public long totalNanosecond;
    }

    [Serializable]
    public class UtcOffset
    {
        public long seconds;
        public long milliseconds;
        public long ticks;
        public long nanoseconds;
    }

    public static class NtpTime
    {
        private static DateTime _responseReceivedTime;
        private static DateTime _ntpDate;
        private static bool _isSynchronized = false;
        private static readonly DateTime _ntpEpoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Added timezone tracking
        private static string _timeZone = string.Empty;
        private static long _utcOffsetSeconds = 0;

        /// <summary>
        /// Ntp date to universal time
        /// </summary>
        /// <returns></returns>
        public static DateTime GetUtcNow()
        {
            if (_isSynchronized)
            {
                // Get current time with elapsed correction
                DateTime currentNtpTime = _ntpDate.AddSeconds(DateTime.Now.Subtract(_responseReceivedTime).TotalSeconds);

                // Convert to UTC based on the stored timezone offset
                DateTime utcTime = currentNtpTime.AddSeconds(-_utcOffsetSeconds);

                return utcTime;
            }
            Logging.PrintWarning<Logger>("[NTP] No synchronized.");
            return DateTime.Now.ToUniversalTime();
        }

        /// <summary>
        /// Ntp date to local time
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNow()
        {
            if (_isSynchronized)
            {
                // First get current time with elapsed correction
                DateTime currentNtpTime = _ntpDate.AddSeconds(DateTime.Now.Subtract(_responseReceivedTime).TotalSeconds);

                // Get the system timezone
                TimeZoneInfo localZone = TimeZoneInfo.Local;

                // Get the system and server timezone offset hours
                double localOffsetHours = localZone.BaseUtcOffset.TotalHours;
                double serverOffsetHours = (double)_utcOffsetSeconds / 3600.0;

                // Check if system timezone is close to the server timezone
                if (Math.Abs(localOffsetHours - serverOffsetHours) < 0.1)
                {
                    // System time zone is practically the same as our server time zone, return as is
                    return currentNtpTime;
                }
                else
                {
                    // System time zone differs from our server time zone
                    // Convert to UTC first by applying the stored offset
                    DateTime utcTime = currentNtpTime.AddSeconds(-_utcOffsetSeconds);
                    // Then convert to system local time
                    return utcTime.ToLocalTime();
                }
            }

            Logging.PrintWarning<Logger>("[NTP] No synchronized.");
            return DateTime.Now.ToLocalTime();
        }

        /// <summary>
        /// Gets the original synchronized time in its original timezone
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNtpDate()
        {
            if (_isSynchronized)
            {
                // Return time in the original timezone it was received in
                return _ntpDate.AddSeconds(DateTime.Now.Subtract(_responseReceivedTime).TotalSeconds);
            }
            Logging.PrintWarning<Logger>("[NTP] No synchronized.");
            return DateTime.Now;
        }

        /// <summary>
        /// Gets the timezone used for the NTP synchronization
        /// </summary>
        /// <returns>Timezone name or empty string if not synchronized</returns>
        public static string GetTimeZone()
        {
            if (_isSynchronized)
            {
                return _timeZone;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the UTC offset in hours for the synchronized time
        /// </summary>
        /// <returns>UTC offset in hours or 0 if not synchronized</returns>
        public static double GetUtcOffset()
        {
            if (_isSynchronized)
            {
                return (double)_utcOffsetSeconds / 3600.0;
            }
            return 0;
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
        /// <para> Ntp Server: time.google.com </para>
        /// <para> Http Request (Format from timeapi.io): https://timeapi.io/api/timezone/zone?timeZone=Asia/Taipei </para>
        /// </summary>
        /// <param name="ntpServer"></param>
        /// <param name="requestTimeout"></param>
        /// <returns></returns>
        public static UniTask Synchronize(string ntpServer = "time.google.com", int requestTimeout = 10)
        {
            _isSynchronized = false;
            return _SynchronizeDate<TimeApiTimezoneResponse>(ntpServer, requestTimeout);
        }

        public static UniTask Synchronize<TResponseFormat>(string ntpServer = "time.google.com", int requestTimeout = 10) where TResponseFormat : TimeApiResponseFormat
        {
            _isSynchronized = false;
            return _SynchronizeDate<TResponseFormat>(ntpServer, requestTimeout);
        }

        private static async UniTask _SynchronizeDate<TResponseFormat>(string ntpServer, int requestTimeout) where TResponseFormat : TimeApiResponseFormat
        {
            if (!_ConnectionEnabled())
            {
                Logging.PrintWarning<Logger>("[NTP] Network not reachable.");
                return;
            }

            var cts = new System.Threading.CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(requestTimeout));

            try
            {
                // WebGL Compatible request method
                await _RequestTimeAsync<TResponseFormat>(ntpServer, requestTimeout, cts.Token);

                // Wait until is synchronized with timeout handling
                try
                {
                    await UniTask.WaitUntil(IsSynchronized, PlayerLoopTiming.FixedUpdate, cts.Token);
                }
                catch (OperationCanceledException ex)
                {
                    if (ex.CancellationToken == cts.Token)
                    {
                        Logging.PrintError<Logger>("[NTP] Sync failed due to timeout.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.PrintError<Logger>($"[NTP] Error: {ex.Message}");
            }
        }

        private static async UniTask _RequestTimeAsync<TResponseFormat>(string ntpServer, int requestTimeout, System.Threading.CancellationToken cancellationToken) where TResponseFormat : TimeApiResponseFormat
        {
            Logging.Print<Logger>($"[NTP] Request started. Attempting to synchronize: {ntpServer}");

            // Try to use provided ntpServer if it's a custom time API
            // Determine if we're dealing with a standard NTP server or a HTTP time API
            bool isHttpApi = ntpServer.StartsWith("http://") || ntpServer.StartsWith("https://");

            if (isHttpApi)
            {
                // If user provided a HTTP/HTTPS URL, use it directly
                string url = ntpServer;
                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    request.timeout = requestTimeout;
                    await request.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        string response = request.downloadHandler.text;
                        _ProcessTimeApiResponse<TResponseFormat>(response);
                    }
                    else
                    {
                        Logging.PrintError<Logger>($"[NTP] Request failed: {request.error}");
                    }
                }
            }
            // For non-WebGL platforms, use socket-based implementation
            else
            {
                // Switch to other thread
                await UniTask.SwitchToThreadPool();

                // Request ntp server
                _SocketRequestNtpTime(ntpServer, requestTimeout);

                try
                {
                    // Wait until is synchronized with timeout handling
                    await UniTask.WaitUntil(IsSynchronized, PlayerLoopTiming.FixedUpdate, cancellationToken);
                }
                catch (OperationCanceledException ex)
                {
                    if (ex.CancellationToken == cancellationToken)
                    {
                        Logging.PrintError<Logger>("[NTP] Sync failed due to timeout.");
                    }
                }

                // Must switch back to main thread
                await UniTask.SwitchToMainThread();
            }
        }

        private static void _ProcessTimeApiResponse<TResponseFormat>(string jsonResponse) where TResponseFormat : TimeApiResponseFormat
        {
            try
            {
                var data = JsonUtility.FromJson<TResponseFormat>(jsonResponse);
                if (data == null)
                {
                    Logging.PrintError<Logger>("[NTP] Failed to parse time response: deserialized to null");
                    return;
                }

                // TimeAPI.io Timezone API (our primary API)
                if (data is TimeApiTimezoneResponse typeData)
                {
                    if (!string.IsNullOrEmpty(typeData.currentLocalTime))
                    {
                        _responseReceivedTime = DateTime.Now;

                        // Parse the local time
                        DateTime localTime = DateTime.Parse(typeData.currentLocalTime);

                        // Store the UTC offset in seconds
                        var utcOffsetSeconds = typeData.currentUtcOffset.seconds;

                        // Store both the local time and UTC offset for accurate conversions
                        _ntpDate = localTime;

                        // Store the timezone info
                        _timeZone = typeData.timeZone;
                        _utcOffsetSeconds = utcOffsetSeconds;

                        _isSynchronized = true;
                        Logging.PrintInfo<Logger>($"[NTP] Date is synchronized using TimeAPI.io Timezone API. " + $"Timezone: {_timeZone}, UTC Offset: {_utcOffsetSeconds / 3600} hours.");
                        return;
                    }
                }

                Logging.PrintError<Logger>(">[NTP] Failed to parse time response: Unknown format");
                Logging.PrintError<Logger>($"Response received: {jsonResponse.Substring(0, Math.Min(100, jsonResponse.Length))}...");
            }
            catch (Exception ex)
            {
                Logging.PrintError<Logger>($"[NTP] Failed to parse time response: {ex.Message}");
                Logging.PrintError<Logger>($"Response was: {jsonResponse.Substring(0, Math.Min(100, jsonResponse.Length))}...");
            }
        }

        private static void _SocketRequestNtpTime(string ntpServer, int requestTimeout)
        {
            // Legacy socket implementation for non-WebGL platforms
            var ntpData = new byte[48];
            // LI = 0, Version = 3, Mode = 3 (client)
            ntpData[0] = 0x1B;

            try
            {
                var addresses = System.Net.Dns.GetHostEntry(ntpServer).AddressList;
                var ipEndPoint = new System.Net.IPEndPoint(addresses[0], 123);

                using (var socket = new System.Net.Sockets.Socket(
                    System.Net.Sockets.AddressFamily.InterNetwork,
                    System.Net.Sockets.SocketType.Dgram,
                    System.Net.Sockets.ProtocolType.Udp))
                {
                    socket.Connect(ipEndPoint);
                    socket.ReceiveTimeout = requestTimeout * 1000;
                    socket.Send(ntpData);
                    socket.Receive(ntpData);

                    _ProcessResponse(ntpData);
                }
            }
            catch (Exception ex)
            {
                Logging.PrintError<Logger>($"[NTP] Socket sync failed: {ex.Message}");
                return;
            }
        }

        private static void _ProcessResponse(byte[] ntpData)
        {
            Logging.Print<Logger>("[NTP] Response received.");

            _responseReceivedTime = DateTime.Now;

            // Big-Endian
            var intPart = ((ulong)ntpData[40] << 24) | ((ulong)ntpData[41] << 16) | ((ulong)ntpData[42] << 8) | ntpData[43];
            var fractPart = ((ulong)ntpData[44] << 24) | ((ulong)ntpData[45] << 16) | ((ulong)ntpData[46] << 8) | ntpData[47];

            var milliseconds = intPart * 1000 + fractPart * 1000 / 0x100000000L;
            _ntpDate = _ntpEpoch.AddMilliseconds((long)milliseconds);

            _isSynchronized = true;

            Logging.PrintInfo<Logger>("[NTP] Date is synchronized via NTP.");
        }

        private static bool _ConnectionEnabled()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}
using Cysharp.Threading.Tasks;
using OxGKit.LoggingSystem;
using OxGKit.Utilities.Cacher;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace OxGKit.Utilities.Requester
{
    public struct ErrorInfo
    {
        public string url;
        public string message;
        public Exception exception;
    }

    public class Requester
    {
        private ARCCache<string, AudioClip> _arcAudios = null;
        private ARCCache<string, Texture2D> _arcTexture2ds = null;
        private ARCCache<string, string> _arcTexts = null;
        private LRUCache<string, AudioClip> _lruAudios = null;
        private LRUCache<string, Texture2D> _lruTexture2ds = null;
        private LRUCache<string, string> _lruTexts = null;

        private const int _MAX_REQUEST_TIME_SECONDS = 180;

        private static readonly object _locker = new object();
        private static Requester _instance;

        internal static Requester GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                        _instance = new Requester();
                }
            }
            return _instance;
        }

        #region ARC Audio
        public static void InitARCCacheCapacityForAudio(int capacity = 20)
        {
            GetInstance().SelfInitARCCacheCapacityForAudio(capacity);
        }

        public void SelfInitARCCacheCapacityForAudio(int capacity = 20)
        {
            if (this._arcAudios == null)
                this._arcAudios = new ARCCache<string, AudioClip>(capacity);
            else
            {
                this.SelfClearARCCacheCapacityForAudio();
                this._arcAudios = new ARCCache<string, AudioClip>(capacity);
            }

            // Only allow one cache type (Clear LRU)
            this.SelfClearLRUCacheCapacityForAudio();
            this._lruAudios = null;
        }

        public static bool RemoveFromARCCacheForAudio(string url)
        {
            return GetInstance().SelfRemoveFromARCCacheForAudio(url);
        }

        public bool SelfRemoveFromARCCacheForAudio(string url)
        {
            if (this._arcAudios != null)
                return this._arcAudios.Remove(url);
            return false;
        }

        public static void ClearARCCacheCapacityForAudio()
        {
            GetInstance().SelfClearARCCacheCapacityForAudio();
        }

        public void SelfClearARCCacheCapacityForAudio()
        {
            if (this._arcAudios != null)
                this._arcAudios.Clear();
        }
        #endregion

        #region ARC Texture2d
        public static void InitARCCacheCapacityForTexture2d(int capacity = 60)
        {
            GetInstance().SelfInitARCCacheCapacityForTexture2d(capacity);
        }

        public void SelfInitARCCacheCapacityForTexture2d(int capacity = 60)
        {
            if (this._arcTexture2ds == null)
                this._arcTexture2ds = new ARCCache<string, Texture2D>(capacity);
            else
            {
                this.SelfClearARCCacheCapacityForTexture2d();
                this._arcTexture2ds = new ARCCache<string, Texture2D>(capacity);
            }

            // Only allow one cache type (Clear LRU)
            this.SelfClearLRUCacheCapacityForTexture2d();
            this._lruTexture2ds = null;
        }

        public static bool RemoveFromARCCacheForTexture2d(string url)
        {
            return GetInstance().SelfRemoveFromARCCacheForTexture2d(url);
        }

        public bool SelfRemoveFromARCCacheForTexture2d(string url)
        {
            if (this._arcTexture2ds != null)
                return this._arcTexture2ds.Remove(url);
            return false;
        }

        public static void ClearARCCacheCapacityForTexture2d()
        {
            GetInstance().SelfClearARCCacheCapacityForTexture2d();
        }

        public void SelfClearARCCacheCapacityForTexture2d()
        {
            if (this._arcTexture2ds != null)
                this._arcTexture2ds.Clear();
        }
        #endregion

        #region ARC Text
        public static void InitARCCacheCapacityForText(int capacity = 100)
        {
            GetInstance().SelfInitARCCacheCapacityForText(capacity);
        }

        public void SelfInitARCCacheCapacityForText(int capacity = 100)
        {
            if (this._arcTexts == null)
                this._arcTexts = new ARCCache<string, string>(capacity);
            else
            {
                this.SelfClearARCCacheCapacityForText();
                this._arcTexts = new ARCCache<string, string>(capacity);
            }

            // Only allow one cache type (Clear LRU)
            this.SelfClearLRUCacheCapacityForText();
            this._lruTexts = null;
        }

        public static bool RemoveFromARCCacheForText(string url)
        {
            return GetInstance().SelfRemoveFromARCCacheForText(url);
        }

        public bool SelfRemoveFromARCCacheForText(string url)
        {
            if (this._arcTexts != null)
                return this._arcTexts.Remove(url);
            return false;
        }

        public static void ClearARCCacheCapacityForText()
        {
            GetInstance().SelfClearARCCacheCapacityForText();
        }

        public void SelfClearARCCacheCapacityForText()
        {
            if (this._arcTexts != null)
                this._arcTexts.Clear();
        }
        #endregion

        #region LRU Audio
        public static void InitLRUCacheCapacityForAudio(int capacity = 20)
        {
            GetInstance().SelfInitLRUCacheCapacityForAudio(capacity);
        }

        public void SelfInitLRUCacheCapacityForAudio(int capacity = 20)
        {
            if (this._lruAudios == null)
                this._lruAudios = new LRUCache<string, AudioClip>(capacity);
            else
            {
                this.SelfClearLRUCacheCapacityForAudio();
                this._lruAudios = new LRUCache<string, AudioClip>(capacity);
            }

            // Only allow one cache type (Clear ARC)
            this.SelfClearARCCacheCapacityForAudio();
            this._arcAudios = null;
        }

        public static bool RemoveFromLRUCacheForAudio(string url)
        {
            return GetInstance().SelfRemoveFromLRUCacheForAudio(url);
        }

        public bool SelfRemoveFromLRUCacheForAudio(string url)
        {
            if (this._lruAudios != null)
                return this._lruAudios.Remove(url);
            return false;
        }

        public static void ClearLRUCacheCapacityForAudio()
        {
            GetInstance().SelfClearLRUCacheCapacityForAudio();
        }

        public void SelfClearLRUCacheCapacityForAudio()
        {
            if (this._lruAudios != null)
                this._lruAudios.Clear();
        }
        #endregion

        #region LRU Texture2d
        public static void InitLRUCacheCapacityForTexture2d(int capacity = 60)
        {
            GetInstance().SelfInitLRUCacheCapacityForTexture2d(capacity);
        }

        public void SelfInitLRUCacheCapacityForTexture2d(int capacity = 60)
        {
            if (this._lruTexture2ds == null)
                this._lruTexture2ds = new LRUCache<string, Texture2D>(capacity);
            else
            {
                this.SelfClearLRUCacheCapacityForTexture2d();
                this._lruTexture2ds = new LRUCache<string, Texture2D>(capacity);
            }

            // Only allow one cache type (Clear ARC)
            this.SelfClearARCCacheCapacityForTexture2d();
            this._arcTexture2ds = null;
        }

        public static bool RemoveFromLRUCacheForTexture2d(string url)
        {
            return GetInstance().SelfRemoveFromLRUCacheForTexture2d(url);
        }

        public bool SelfRemoveFromLRUCacheForTexture2d(string url)
        {
            if (this._lruTexture2ds != null)
                return this._lruTexture2ds.Remove(url);
            return false;
        }

        public static void ClearLRUCacheCapacityForTexture2d()
        {
            GetInstance().SelfClearLRUCacheCapacityForTexture2d();
        }

        public void SelfClearLRUCacheCapacityForTexture2d()
        {
            if (this._lruTexture2ds != null)
                this._lruTexture2ds.Clear();
        }
        #endregion

        #region LRU Text
        public static void InitLRUCacheCapacityForText(int capacity = 80)
        {
            GetInstance().SelfInitLRUCacheCapacityForText(capacity);
        }

        public void SelfInitLRUCacheCapacityForText(int capacity = 80)
        {
            if (this._lruTexts == null)
                this._lruTexts = new LRUCache<string, string>(capacity);
            else
            {
                this.SelfClearLRUCacheCapacityForText();
                this._lruTexts = new LRUCache<string, string>(capacity);
            }

            // Only allow one cache type (Clear ARC)
            this.SelfClearARCCacheCapacityForText();
            this._arcTexts = null;
        }

        public static bool RemoveFromLRUCacheForText(string url)
        {
            return GetInstance().SelfRemoveFromLRUCacheForText(url);
        }

        public bool SelfRemoveFromLRUCacheForText(string url)
        {
            if (this._lruTexts != null)
                return this._lruTexts.Remove(url);
            return false;
        }

        public static void ClearLRUCacheCapacityForText()
        {
            GetInstance().SelfClearLRUCacheCapacityForText();
        }

        public void SelfClearLRUCacheCapacityForText()
        {
            if (this._lruTexts != null)
                this._lruTexts.Clear();
        }
        #endregion

        public static bool AutoRemoveFromCaches(string url)
        {
            return GetInstance().SelfAutoRemoveFromCaches(url);
        }

        /// <summary>
        /// Searching all caches and remove it
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool SelfAutoRemoveFromCaches(string url)
        {
            do
            {
                bool finished = this.SelfRemoveFromARCCacheForAudio(url);
                if (finished)
                    return true;
                finished = this.SelfRemoveFromARCCacheForTexture2d(url);
                if (finished)
                    return true;
                finished = this.SelfRemoveFromARCCacheForText(url);
                if (finished)
                    return true;
                finished = this.SelfRemoveFromLRUCacheForAudio(url);
                if (finished)
                    return true;
                finished = this.SelfRemoveFromLRUCacheForTexture2d(url);
                if (finished)
                    return true;
                finished = this.SelfRemoveFromLRUCacheForText(url);
                if (finished)
                    return true;
                return false;
            } while (true);
        }

        public static void ClearAllCaches()
        {
            GetInstance().SelfClearAllCaches();
        }

        public static void Release()
        {
            GetInstance().SelfRelease();
        }

        public void SelfClearAllCaches()
        {
            this.SelfClearARCCacheCapacityForAudio();
            this.SelfClearARCCacheCapacityForText();
            this.SelfClearARCCacheCapacityForTexture2d();
            this.SelfClearLRUCacheCapacityForAudio();
            this.SelfClearLRUCacheCapacityForText();
            this.SelfClearLRUCacheCapacityForTexture2d();
        }

        public void SelfRelease()
        {
            this.SelfClearAllCaches();
            this._arcAudios = null;
            this._arcTexture2ds = null;
            this._arcTexts = null;
            this._lruAudios = null;
            this._lruTexture2ds = null;
            this._lruTexts = null;
        }

        /// <summary>
        /// Audio reques
        /// </summary>
        /// <param name="url"></param>
        /// <param name="audioType"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="cts"></param>
        /// <param name="cached"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static async UniTask<AudioClip> RequestAudio(string url, AudioType audioType = AudioType.MPEG, Action<AudioClip> successAction = null, Action<ErrorInfo> errorAction = null, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            return await GetInstance().SelfRequestAudio(url, audioType, successAction, errorAction, cts, cached, timeoutSeconds);
        }

        /// <summary>
        /// Audio request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="audioType"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="cts"></param>
        /// <param name="cached"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public async UniTask<AudioClip> SelfRequestAudio(string url, AudioType audioType = AudioType.MPEG, Action<AudioClip> successAction = null, Action<ErrorInfo> errorAction = null, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            if (CheckUrlMissing(url, errorAction))
                return null;

            AudioClip audioClip = null;

            if (cached)
            {
                // ARCCache
                if (this._arcAudios != null)
                {
                    audioClip = this._arcAudios.Get(url);
                }
                // LRUCache
                else if (this._lruAudios != null)
                {
                    audioClip = this._lruAudios.Get(url);
                }
            }

            if (audioClip != null)
            {
                successAction?.Invoke(audioClip);
                return audioClip;
            }

            audioClip = await SendRequest<AudioClip>
            (
                url,
                cts,
                timeoutSeconds,
                () =>
                {
                    UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
                    ((DownloadHandlerAudioClip)req.downloadHandler).streamAudio = true;
                    return req;
                },
                req => ((DownloadHandlerAudioClip)req.downloadHandler).audioClip,
                errorAction
            );

            if (cached && audioClip != null)
            {
                // ARCCache
                if (this._arcAudios != null)
                {
                    this._arcAudios.Add(url, audioClip);
                    audioClip = this._arcAudios.Get(url);
                }
                // LRUCache
                else if (this._lruAudios != null)
                {
                    this._lruAudios.Add(url, audioClip);
                    audioClip = this._lruAudios.Get(url);
                }
            }

            if (audioClip != null)
                successAction?.Invoke(audioClip);

            return audioClip;
        }

        /// <summary>
        /// Texture2D request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="cts"></param>
        /// <param name="cached"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static async UniTask<Texture2D> RequestTexture2D(string url, Action<Texture2D> successAction = null, Action<ErrorInfo> errorAction = null, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            return await GetInstance().SelfRequestTexture2D(url, successAction, errorAction, cts, cached, timeoutSeconds);
        }

        /// <summary>
        /// Texture2D request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="cts"></param>
        /// <param name="cached"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public async UniTask<Texture2D> SelfRequestTexture2D(string url, Action<Texture2D> successAction = null, Action<ErrorInfo> errorAction = null, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            if (CheckUrlMissing(url, errorAction))
                return null;

            Texture2D t2d = null;

            if (cached)
            {
                // ARCCache
                if (this._arcTexture2ds != null)
                {
                    t2d = this._arcTexture2ds.Get(url);
                }
                // LRUCache
                else if (this._lruTexture2ds != null)
                {
                    t2d = this._lruTexture2ds.Get(url);
                }
            }

            if (t2d != null)
            {
                successAction?.Invoke(t2d);
                return t2d;
            }

            t2d = await SendRequest<Texture2D>
            (
                url,
                cts,
                timeoutSeconds,
                () =>
                {
                    UnityWebRequest req = new UnityWebRequest(url);
                    req.downloadHandler = new DownloadHandlerTexture();
                    return req;
                },
                req => ((DownloadHandlerTexture)req.downloadHandler).texture,
                errorAction
            );

            if (cached && t2d != null)
            {
                // ARCCache
                if (this._arcTexture2ds != null)
                {
                    this._arcTexture2ds.Add(url, t2d);
                    t2d = this._arcTexture2ds.Get(url);
                }
                // LRUCache
                else if (this._lruTexture2ds != null)
                {
                    this._lruTexture2ds.Add(url, t2d);
                    t2d = this._lruTexture2ds.Get(url);
                }
            }

            if (t2d != null)
                successAction?.Invoke(t2d);

            return t2d;
        }

        /// <summary>
        /// Sprite request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="position"></param>
        /// <param name="pivot"></param>
        /// <param name="pixelPerUnit"></param>
        /// <param name="extrude"></param>
        /// <param name="meshType"></param>
        /// <param name="cts"></param>
        /// <param name="cached"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static async UniTask<Sprite> RequestSprite(string url, Action<Sprite> successAction = null, Action<ErrorInfo> errorAction = null, Vector2 position = default, Vector2 pivot = default, float pixelPerUnit = 100, uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.FullRect, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            return await GetInstance().SelfRequestSprite(url, successAction, errorAction, position, pivot, pixelPerUnit, extrude, meshType, cts, cached, timeoutSeconds);
        }

        /// <summary>
        /// Sprite request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="position"></param>
        /// <param name="pivot"></param>
        /// <param name="pixelPerUnit"></param>
        /// <param name="extrude"></param>
        /// <param name="meshType"></param>
        /// <param name="cts"></param>
        /// <param name="cached"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public async UniTask<Sprite> SelfRequestSprite(string url, Action<Sprite> successAction = null, Action<ErrorInfo> errorAction = null, Vector2 position = default, Vector2 pivot = default, float pixelPerUnit = 100, uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.FullRect, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            var t2d = await this.SelfRequestTexture2D(url, null, errorAction, cts, cached, timeoutSeconds);

            if (t2d != null)
            {
                pivot = pivot != Vector2.zero ? pivot : new Vector2(0.5f, 0.5f);
                Sprite sprite = Sprite.Create(t2d, new Rect(position.x, position.y, t2d.width, t2d.height), pivot, pixelPerUnit, extrude, meshType);
                successAction?.Invoke(sprite);
                return sprite;
            }

            return null;
        }

        /// <summary>
        /// File bytes request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="cts"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static async UniTask<byte[]> RequestBytes(string url, Action<byte[]> successAction = null, Action<ErrorInfo> errorAction = null, CancellationTokenSource cts = null, int? timeoutSeconds = null)
        {
            return await GetInstance().SelfRequestBytes(url, successAction, errorAction, cts, timeoutSeconds);
        }

        /// <summary>
        /// File bytes request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="cts"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public async UniTask<byte[]> SelfRequestBytes(string url, Action<byte[]> successAction = null, Action<ErrorInfo> errorAction = null, CancellationTokenSource cts = null, int? timeoutSeconds = null)
        {
            if (CheckUrlMissing(url, errorAction))
                return null;

            byte[] bytes = await SendRequest<byte[]>
            (
                url,
                cts,
                timeoutSeconds,
                () => UnityWebRequest.Get(url),
                req => req.downloadHandler.data,
                errorAction
            );

            if (bytes != null)
                successAction?.Invoke(bytes);

            return bytes;
        }

        public static async UniTask<string> RequestText(string url, Action<string> successAction = null, Action<ErrorInfo> errorAction = null, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            return await GetInstance().SelfRequestText(url, successAction, errorAction, cts, cached, timeoutSeconds);
        }

        /// <summary>
        /// File text request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="cts"></param>
        /// <param name="cached"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public async UniTask<string> SelfRequestText(string url, Action<string> successAction = null, Action<ErrorInfo> errorAction = null, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            if (CheckUrlMissing(url, errorAction))
                return null;

            string text = null;

            if (cached)
            {
                // ARCCache
                if (this._arcTexts != null)
                {
                    text = this._arcTexts.Get(url);
                }
                // LRUCache
                else if (_lruTexts != null)
                {
                    text = this._lruTexts.Get(url);
                }
            }

            if (text != null)
            {
                successAction?.Invoke(text);
                return text;
            }

            text = await SendRequest<string>
            (
                url,
                cts,
                timeoutSeconds,
                () => UnityWebRequest.Get(url),
                req => req.downloadHandler.text,
                errorAction
            );

            if (cached && text != null)
            {
                // ARCCache
                if (this._arcTexts != null)
                {
                    this._arcTexts.Add(url, text);
                    text = this._arcTexts.Get(url);
                }
                // LRUCache
                else if (this._lruTexts != null)
                {
                    this._lruTexts.Add(url, text);
                    text = this._lruTexts.Get(url);
                }
            }

            if (text != null)
                successAction?.Invoke(text);

            return text;
        }

        #region Internal Methods
        /// <summary>
        /// Create UnityWebRequest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="cts"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="createRequest"></param>
        /// <param name="processResponse"></param>
        /// <param name="errorAction"></param>
        /// <returns></returns>
        internal static async UniTask<T> SendRequest<T>(string url, CancellationTokenSource cts, int? timeoutSeconds, Func<UnityWebRequest> createRequest, Func<UnityWebRequest, T> processResponse, Action<ErrorInfo> errorAction)
        {
            UnityWebRequest request = null;
            try
            {
                request = createRequest();

                if (cts != null)
                {
                    await request.SendWebRequest().WithCancellation(cts.Token);
                }
                else
                {
                    timeoutSeconds ??= _MAX_REQUEST_TIME_SECONDS;
                    cts = new CancellationTokenSource();
                    cts.CancelAfterSlim(TimeSpan.FromSeconds((int)timeoutSeconds));
                    await request.SendWebRequest().WithCancellation(cts.Token);
                }

                if (request.result == UnityWebRequest.Result.DataProcessingError ||
                    request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.ConnectionError)
                {
                    var errorInfo = new ErrorInfo();
                    errorInfo.url = url;
                    errorInfo.message = request.error;
                    errorInfo.exception = null;
                    errorAction?.Invoke(errorInfo);
                    request.Dispose();
                    Logging.PrintError<Logger>($"Request failed. URL: {errorInfo.url}, ErrorMsg: {errorInfo.message}");
                    return default;
                }

                T result = processResponse(request);

#if UNITY_EDITOR
                ulong sizeBytes = (ulong)request.downloadHandler.data.Length;
                Logging.Print<Logger>($"Request result => Size: {GetBytesToString(sizeBytes)}");
#endif

                request.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                var errorInfo = new ErrorInfo();
                errorInfo.url = url;
                errorInfo.message = request?.error;
                errorInfo.exception = ex;
                errorAction?.Invoke(errorInfo);
                request?.Dispose();
                Logging.PrintError<Logger>($"Request failed. URL: {errorInfo.url}, ErrorMsg: {errorInfo.message}, Exception: {ex}");
                return default;
            }
        }

        /// <summary>
        /// Bytes ToString (KB, MB, GB)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static string GetBytesToString(ulong bytes)
        {
            if (bytes < (1024 * 1024 * 1f))
            {
                return (bytes / 1024f).ToString("f2") + "KB";
            }
            else if (bytes >= (1024 * 1024 * 1f) && bytes < (1024 * 1024 * 1024 * 1f))
            {
                return (bytes / (1024 * 1024 * 1f)).ToString("f2") + "MB";
            }
            else
            {
                return (bytes / (1024 * 1024 * 1024 * 1f)).ToString("f2") + "GB";
            }
        }

        /// <summary>
        /// Check url state
        /// </summary>
        /// <param name="url"></param>
        /// <param name="errorAction"></param>
        /// <returns></returns>
        internal static bool CheckUrlMissing(string url, Action<ErrorInfo> errorAction)
        {
            if (string.IsNullOrEmpty(url))
            {
                var errorInfo = new ErrorInfo();
                errorInfo.url = null;
                errorInfo.message = "Request failed. URL is null or empty.";
                errorInfo.exception = null;
                Logging.PrintError<Logger>($"{errorInfo.message}");
                errorAction?.Invoke(errorInfo);
                return true;
            }
            return false;
        }
        #endregion
    }
}

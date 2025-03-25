using Cysharp.Threading.Tasks;
using OxGKit.LoggingSystem;
using OxGKit.Utilities.Cacher;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace OxGKit.Utilities.Requester
{
    public static class Requester
    {
        private static ARCCache<string, AudioClip> _arcAudios = null;
        private static ARCCache<string, Texture2D> _arcTexture2ds = null;
        private static ARCCache<string, string> _arcTexts = null;
        private static LRUCache<string, AudioClip> _lruAudios = null;
        private static LRUCache<string, Texture2D> _lruTexture2ds = null;
        private static LRUCache<string, string> _lruTexts = null;

        private const int _MAX_REQUEST_TIME_SECONDS = 180;

        #region ARC Audio
        public static void InitARCCacheCapacityForAudio(int capacity = 20)
        {
            if (_arcAudios == null)
                _arcAudios = new ARCCache<string, AudioClip>(capacity);
            else
            {
                ClearARCCacheCapacityForAudio();
                _arcAudios = new ARCCache<string, AudioClip>(capacity);
            }

            // Only allow one cache type (Clear LRU)
            ClearLRUCacheCapacityForAudio();
            _lruAudios = null;
        }

        public static bool RemoveFromARCCacheForAudio(string url)
        {
            if (_arcAudios != null)
                return _arcAudios.Remove(url);
            return false;
        }

        public static void ClearARCCacheCapacityForAudio()
        {
            if (_arcAudios != null)
                _arcAudios.Clear();
        }
        #endregion

        #region ARC Texture2d
        public static void InitARCCacheCapacityForTexture2d(int capacity = 60)
        {
            if (_arcTexture2ds == null)
                _arcTexture2ds = new ARCCache<string, Texture2D>(capacity);
            else
            {
                ClearARCCacheCapacityForTexture2d();
                _arcTexture2ds = new ARCCache<string, Texture2D>(capacity);
            }

            // Only allow one cache type (Clear LRU)
            ClearLRUCacheCapacityForTexture2d();
            _lruTexture2ds = null;
        }

        public static bool RemoveFromARCCacheForTexture2d(string url)
        {
            if (_arcTexture2ds != null)
                return _arcTexture2ds.Remove(url);
            return false;
        }

        public static void ClearARCCacheCapacityForTexture2d()
        {
            if (_arcTexture2ds != null)
            {
                string[] urls = _arcTexture2ds.GetKeys();
                foreach (var url in urls)
                {
                    RemoveFromARCCacheForTexture2d(url);
                }
                _arcTexture2ds.Clear();
            }
        }
        #endregion

        #region ARC Text
        public static void InitARCCacheCapacityForText(int capacity = 100)
        {
            if (_arcTexts == null)
                _arcTexts = new ARCCache<string, string>(capacity);
            else
            {
                ClearARCCacheCapacityForText();
                _arcTexts = new ARCCache<string, string>(capacity);
            }

            // Only allow one cache type (Clear LRU)
            ClearLRUCacheCapacityForText();
            _lruTexts = null;
        }

        public static bool RemoveFromARCCacheForText(string url)
        {
            if (_arcTexts != null)
                return _arcTexts.Remove(url);
            return false;
        }

        public static void ClearARCCacheCapacityForText()
        {
            if (_arcTexts != null)
                _arcTexts.Clear();
        }
        #endregion

        #region LRU Audio
        public static void InitLRUCacheCapacityForAudio(int capacity = 20)
        {
            if (_lruAudios == null)
                _lruAudios = new LRUCache<string, AudioClip>(capacity);
            else
            {
                ClearLRUCacheCapacityForAudio();
                _lruAudios = new LRUCache<string, AudioClip>(capacity);
            }

            // Only allow one cache type (Clear ARC)
            ClearARCCacheCapacityForAudio();
            _arcAudios = null;
        }

        public static bool RemoveFromLRUCacheForAudio(string url)
        {
            if (_lruAudios != null)
                return _lruAudios.Remove(url);
            return false;
        }

        public static void ClearLRUCacheCapacityForAudio()
        {
            if (_lruAudios != null)
                _lruAudios.Clear();
        }
        #endregion

        #region LRU Texture2d
        public static void InitLRUCacheCapacityForTexture2d(int capacity = 60)
        {
            if (_lruTexture2ds == null)
                _lruTexture2ds = new LRUCache<string, Texture2D>(capacity);
            else
            {
                ClearLRUCacheCapacityForTexture2d();
                _lruTexture2ds = new LRUCache<string, Texture2D>(capacity);
            }

            // Only allow one cache type (Clear ARC)
            ClearARCCacheCapacityForTexture2d();
            _arcTexture2ds = null;
        }

        public static bool RemoveFromLRUCacheForTexture2d(string url)
        {
            if (_lruTexture2ds != null)
                return _lruTexture2ds.Remove(url);
            return false;
        }

        public static void ClearLRUCacheCapacityForTexture2d()
        {
            if (_lruTexture2ds != null)
            {
                string[] urls = _lruTexture2ds.GetKeys();
                foreach (var url in urls)
                {
                    RemoveFromLRUCacheForTexture2d(url);
                }
                _lruTexture2ds.Clear();
            }
        }
        #endregion

        #region LRU Text
        public static void InitLRUCacheCapacityForText(int capacity = 80)
        {
            if (_lruTexts == null)
                _lruTexts = new LRUCache<string, string>(capacity);
            else
            {
                ClearLRUCacheCapacityForText();
                _lruTexts = new LRUCache<string, string>(capacity);
            }

            // Only allow one cache type (Clear ARC)
            ClearARCCacheCapacityForText();
            _arcTexts = null;
        }

        public static bool RemoveFromLRUCacheForText(string url)
        {
            if (_lruTexts != null)
                return _lruTexts.Remove(url);
            return false;
        }

        public static void ClearLRUCacheCapacityForText()
        {
            if (_lruTexts != null)
                _lruTexts.Clear();
        }
        #endregion

        /// <summary>
        /// Searching all caches and remove it
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool AutoRemoveFromCaches(string url)
        {
            do
            {
                bool finished = RemoveFromARCCacheForAudio(url);
                if (finished)
                    return true;
                finished = RemoveFromARCCacheForTexture2d(url);
                if (finished)
                    return true;
                finished = RemoveFromARCCacheForText(url);
                if (finished)
                    return true;
                finished = RemoveFromLRUCacheForAudio(url);
                if (finished)
                    return true;
                finished = RemoveFromLRUCacheForTexture2d(url);
                if (finished)
                    return true;
                finished = RemoveFromLRUCacheForText(url);
                if (finished)
                    return true;
                return false;
            } while (true);
        }

        public static void ClearAllCaches()
        {
            ClearARCCacheCapacityForAudio();
            ClearARCCacheCapacityForText();
            ClearARCCacheCapacityForTexture2d();
            ClearLRUCacheCapacityForAudio();
            ClearLRUCacheCapacityForText();
            ClearLRUCacheCapacityForTexture2d();
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
        public static async UniTask<AudioClip> RequestAudio(string url, AudioType audioType = AudioType.MPEG, Action<AudioClip> successAction = null, Action<string> errorAction = null, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                Logging.PrintWarning<Logger>($"<color=#FF0000>Request failed. URL is null or empty.</color>");
                return null;
            }

            if (cached)
            {
                // ARCCache
                if (_arcAudios != null)
                {
                    AudioClip audioClip = _arcAudios.Get(url);
                    if (audioClip != null)
                        return audioClip;
                }
                // LRUCache
                else if (_lruAudios != null)
                {
                    AudioClip audioClip = _lruAudios.Get(url);
                    if (audioClip != null)
                        return audioClip;
                }
            }

            AudioClip audioClipResult = await SendRequest<AudioClip>
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

            if (cached && audioClipResult != null)
            {
                // ARCCache
                if (_arcAudios != null)
                {
                    _arcAudios.Add(url, audioClipResult);
                    audioClipResult = _arcAudios.Get(url);
                }
                // LRUCache
                else if (_lruAudios != null)
                {
                    _lruAudios.Add(url, audioClipResult);
                    audioClipResult = _lruAudios.Get(url);
                }
            }

            successAction?.Invoke(audioClipResult);
            return audioClipResult;
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
        public static async UniTask<Texture2D> RequestTexture2D(string url, Action<Texture2D> successAction = null, Action<string> errorAction = null, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                Logging.PrintWarning<Logger>($"<color=#FF0000>Request failed. URL is null or empty.</color>");
                return null;
            }

            if (cached)
            {
                // ARCCache
                if (_arcTexture2ds != null)
                {
                    Texture2D t2d = _arcTexture2ds.Get(url);
                    if (t2d != null)
                        return t2d;
                }
                // LRUCache
                else if (_lruTexture2ds != null)
                {
                    Texture2D t2d = _lruTexture2ds.Get(url);
                    if (t2d != null)
                        return t2d;
                }
            }

            Texture2D textureResult = await SendRequest<Texture2D>
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

            if (cached && textureResult != null)
            {
                // ARCCache
                if (_arcTexture2ds != null)
                {
                    _arcTexture2ds.Add(url, textureResult);
                    textureResult = _arcTexture2ds.Get(url);
                }
                // LRUCache
                else if (_lruTexture2ds != null)
                {
                    _lruTexture2ds.Add(url, textureResult);
                    textureResult = _lruTexture2ds.Get(url);
                }
            }

            successAction?.Invoke(textureResult);
            return textureResult;
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
        public static async UniTask<Sprite> RequestSprite(string url, Action<Sprite> successAction = null, Action<string> errorAction = null, Vector2 position = default, Vector2 pivot = default, float pixelPerUnit = 100, uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.FullRect, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            var texture = await RequestTexture2D(url, null, errorAction, cts, cached, timeoutSeconds);
            if (texture != null)
            {
                pivot = pivot != Vector2.zero ? pivot : new Vector2(0.5f, 0.5f);
                Sprite sprite = Sprite.Create(texture, new Rect(position.x, position.y, texture.width, texture.height), pivot, pixelPerUnit, extrude, meshType);
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
        public static async UniTask<byte[]> RequestBytes(string url, Action<byte[]> successAction = null, Action<string> errorAction = null, CancellationTokenSource cts = null, int? timeoutSeconds = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                Logging.PrintWarning<Logger>($"<color=#FF0000>Request failed. URL is null or empty.</color>");
                return null;
            }

            byte[] bytesResult = await SendRequest<byte[]>
            (
                url,
                cts,
                timeoutSeconds,
                () => UnityWebRequest.Get(url),
                req => req.downloadHandler.data,
                errorAction
            );

            successAction?.Invoke(bytesResult);
            return bytesResult;
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
        public static async UniTask<string> RequestText(string url, Action<string> successAction = null, Action<string> errorAction = null, CancellationTokenSource cts = null, bool cached = true, int? timeoutSeconds = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                Logging.PrintWarning<Logger>($"<color=#FF0000>Request failed. URL is null or empty.</color>");
                return null;
            }

            if (cached)
            {
                // ARCCache
                if (_arcTexts != null)
                {
                    string text = _arcTexts.Get(url);
                    if (text != null)
                        return text;
                }
                // LRUCache
                else if (_lruTexts != null)
                {
                    string text = _lruTexts.Get(url);
                    if (text != null)
                        return text;
                }
            }

            string textResult = await SendRequest<string>
            (
                url,
                cts,
                timeoutSeconds,
                () => UnityWebRequest.Get(url),
                req => req.downloadHandler.text,
                errorAction
            );

            if (cached && textResult != null)
            {
                // ARCCache
                if (_arcTexts != null)
                {
                    _arcTexts.Add(url, textResult);
                    textResult = _arcTexts.Get(url);
                }
                // LRUCache
                else if (_lruTexts != null)
                {
                    _lruTexts.Add(url, textResult);
                    textResult = _lruTexts.Get(url);
                }
            }

            successAction?.Invoke(textResult);
            return textResult;
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
        internal static async UniTask<T> SendRequest<T>(string url, CancellationTokenSource cts, int? timeoutSeconds, Func<UnityWebRequest> createRequest, Func<UnityWebRequest, T> processResponse, Action<string> errorAction)
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
                    string errorMsg = request.error;
                    errorAction?.Invoke(errorMsg);
                    request.Dispose();
                    Logging.PrintWarning<Logger>($"<color=#FF0000>Request failed. URL: {url}</color>");
                    return default;
                }

                T result = processResponse(request);

#if UNITY_EDITOR
                ulong sizeBytes = (ulong)request.downloadHandler.data.Length;
                Logging.Print<Logger>($"<color=#90ff67>Request result => Size: {GetBytesToString(sizeBytes)}</color>");
#endif

                request.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                string errorMsg = string.IsNullOrEmpty(request?.error) ? $"RequestAPI failed. URL: {url}, Exception: {ex}" : request.error;
                errorAction?.Invoke(errorMsg);
                request?.Dispose();
                Logging.PrintWarning<Logger>($"<color=#FF0000>{errorMsg}</color>");
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
        #endregion
    }
}

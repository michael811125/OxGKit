using Cysharp.Threading.Tasks;
using OxGKit.Utilities.Cacher;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace OxGKit.Utilities.Request
{
    public static class Requester
    {
        private static ARCCache<string, AudioClip> _arcAudios = null;
        private static ARCCache<string, Texture2D> _arcTexture2ds = null;
        private static ARCCache<string, string> _arcTexts = null;
        private static LRUCache<string, AudioClip> _lruAudios = null;
        private static LRUCache<string, Texture2D> _lruTexture2ds = null;
        private static LRUCache<string, string> _lruTexts = null;

        #region ARC Audio
        public static void InitARCCacheCapacityForAudio(int capacity = 20)
        {
            if (_arcAudios == null) _arcAudios = new ARCCache<string, AudioClip>(capacity);
            if (_lruAudios != null)
            {
                _lruAudios.Clear();
                // Only allow one cache type
                _lruAudios = null;
            }
        }

        public static bool RemoveFromARCCacheForAudio(string url)
        {
            if (_arcAudios != null) return _arcAudios.Remove(url);
            return false;
        }

        public static void ClearARCCacheCapacityForAudio()
        {
            if (_arcAudios != null) _arcAudios.Clear();
        }
        #endregion

        #region ARC Texture2d
        public static void InitARCCacheCapacityForTexture2d(int capacity = 60)
        {
            if (_arcTexture2ds == null) _arcTexture2ds = new ARCCache<string, Texture2D>(capacity);
            if (_lruTexture2ds != null)
            {
                _lruTexture2ds.Clear();
                // Only allow one cache type
                _lruTexture2ds = null;
            }
        }

        public static bool RemoveFromARCCacheForTexture2d(string url)
        {
            if (_arcTexture2ds != null) return _arcTexture2ds.Remove(url);
            return false;
        }

        public static void ClearARCCacheCapacityForTexture2d()
        {
            if (_arcTexture2ds != null) _arcTexture2ds.Clear();
        }
        #endregion

        #region ARC Text
        public static void InitARCCacheCapacityForText(int capacity = 100)
        {
            if (_arcTexts == null) _arcTexts = new ARCCache<string, string>(capacity);
            if (_lruTexts != null)
            {
                _lruTexts.Clear();
                // Only allow one cache type
                _lruTexts = null;
            }
        }

        public static bool RemoveFromARCCacheForText(string url)
        {
            if (_arcTexts != null) return _arcTexts.Remove(url);
            return false;
        }

        public static void ClearARCCacheCapacityForText()
        {
            if (_arcTexts != null) _arcTexts.Clear();
        }
        #endregion

        #region LRU Audio
        public static void InitLRUCacheCapacityForAudio(int capacity = 20)
        {
            if (_lruAudios == null) _lruAudios = new LRUCache<string, AudioClip>(capacity);
            if (_arcAudios != null)
            {
                _arcAudios.Clear();
                // Only allow one cache type
                _arcAudios = null;
            }
        }

        public static bool RemoveFromLRUCacheForAudio(string url)
        {
            if (_lruAudios != null) return _lruAudios.Remove(url);
            return false;
        }

        public static void ClearLRUCacheCapacityForAudio()
        {
            if (_lruAudios != null) _lruAudios.Clear();
        }
        #endregion

        #region LRU Texture2d
        public static void InitLRUCacheCapacityForTexture2d(int capacity = 60)
        {
            if (_lruTexture2ds == null) _lruTexture2ds = new LRUCache<string, Texture2D>(capacity);
            if (_arcTexture2ds != null)
            {
                _arcTexture2ds.Clear();
                // Only allow one cache type
                _arcTexture2ds = null;
            }
        }

        public static bool RemoveFromLRUCacheForTexture2d(string url)
        {
            if (_lruTexture2ds != null) return _lruTexture2ds.Remove(url);
            return false;
        }

        public static void ClearLRUCacheCapacityForTexture2d()
        {
            if (_lruTexture2ds != null) _lruTexture2ds.Clear();
        }
        #endregion

        #region LRU Text
        public static void InitLRUCacheCapacityForText(int capacity = 80)
        {
            if (_lruTexts == null) _lruTexts = new LRUCache<string, string>(capacity);
            if (_arcTexts != null)
            {
                _arcTexts.Clear();
                // Only allow one cache type
                _arcTexts = null;
            }
        }

        public static bool RemoveFromLRUCacheForText(string url)
        {
            if (_lruTexts != null) return _lruTexts.Remove(url);
            return false;
        }

        public static void ClearLRUCacheCapacityForText()
        {
            if (_lruTexts != null) _lruTexts.Clear();
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
                if (finished) return true;
                finished = RemoveFromARCCacheForTexture2d(url);
                if (finished) return true;
                finished = RemoveFromARCCacheForText(url);
                if (finished) return true;
                finished = RemoveFromLRUCacheForAudio(url);
                if (finished) return true;
                finished = RemoveFromLRUCacheForTexture2d(url);
                if (finished) return true;
                finished = RemoveFromLRUCacheForText(url);
                if (finished) return true;
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
        /// <returns></returns>
        public static async UniTask<AudioClip> RequestAudio(string url, AudioType audioType = AudioType.MPEG, Action<AudioClip> successAction = null, Action errorAction = null, CancellationTokenSource cts = null, bool cached = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.Log($"<color=#FF0000>Request failed, URL is null or empty.</color>");
                return null;
            }

            if (cached)
            {
                // ARCCache
                if (_arcAudios != null)
                {
                    AudioClip audioClip = _arcAudios.Get(url);
                    if (audioClip != null) return audioClip;
                }
                // LRUCache
                else if (_lruAudios != null)
                {
                    AudioClip audioClip = _lruAudios.Get(url);
                    if (audioClip != null) return audioClip;
                }
            }

            UnityWebRequest request = null;
            try
            {
                request = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
                ((DownloadHandlerAudioClip)request.downloadHandler).streamAudio = true;

                if (cts != null) await request.SendWebRequest().WithCancellation(cts.Token);
                else await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.ConnectionError)
                {
                    request.Dispose();
                    errorAction?.Invoke();
                    Debug.Log($"<color=#FF0000>Request failed, URL: {url}</color>");
                    return null;
                }

                AudioClip audioClip = ((DownloadHandlerAudioClip)request.downloadHandler).audioClip;
                if (cached)
                {
                    // ARCCache
                    if (_arcAudios != null)
                    {
                        _arcAudios.Add(url, audioClip);
                        audioClip = _arcAudios.Get(url);
                    }
                    // LRUCache
                    else if (_lruAudios != null)
                    {
                        _lruAudios.Add(url, audioClip);
                        audioClip = _lruAudios.Get(url);
                    }
                }
                successAction?.Invoke(audioClip);

#if UNITY_EDITOR
                ulong sizeBytes = (ulong)request.downloadHandler.data.Length;
                Debug.Log($"<color=#90ff67>Request Audio => Channel: {audioClip.channels}, Frequency: {audioClip.frequency}, Sample: {audioClip.samples}, Length: {audioClip.length}, State: {audioClip.loadState}, Size: {GetBytesToString(sizeBytes)}</color>");
#endif

                request.Dispose();
                return audioClip;
            }
            catch
            {
                request.Dispose();
                errorAction?.Invoke();
                Debug.Log($"<color=#FF0000>Request failed, URL: {url}</color>");
                return null;
            }
        }

        /// <summary>
        /// Texture2D request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public static async UniTask<Texture2D> RequestTexture2D(string url, Action<Texture2D> successAction = null, Action errorAction = null, CancellationTokenSource cts = null, bool cached = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.Log($"<color=#FF0000>Request failed, URL is null or empty.</color>");
                return null;
            }

            if (cached)
            {
                // ARCCache
                if (_arcTexture2ds != null)
                {
                    Texture2D t2d = _arcTexture2ds.Get(url);
                    if (t2d != null) return t2d;
                }
                // LRUCache
                else if (_lruTexture2ds != null)
                {
                    Texture2D t2d = _lruTexture2ds.Get(url);
                    if (t2d != null) return t2d;
                }
            }

            UnityWebRequest request = null;
            try
            {
                request = new UnityWebRequest(url);
                request.downloadHandler = new DownloadHandlerTexture();

                if (cts != null) await request.SendWebRequest().WithCancellation(cts.Token);
                else await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.ConnectionError)
                {
                    request.Dispose();
                    errorAction?.Invoke();
                    Debug.Log($"<color=#FF0000>Request failed, URL: {url}</color>");
                    return null;
                }

                Texture2D t2d = ((DownloadHandlerTexture)request.downloadHandler).texture;
                if (cached)
                {
                    // ARCCache
                    if (_arcTexture2ds != null)
                    {
                        _arcTexture2ds.Add(url, t2d);
                        t2d = _arcTexture2ds.Get(url);
                    }
                    // LRUCache
                    else if (_lruTexture2ds != null)
                    {
                        _lruTexture2ds.Add(url, t2d);
                        t2d = _lruTexture2ds.Get(url);
                    }
                }
                successAction?.Invoke(t2d);

#if UNITY_EDITOR
                ulong sizeBytes = (ulong)request.downloadHandler.data.Length;
                Debug.Log($"<color=#90ff67>Request Texture2D => Width: {t2d.width}, Height: {t2d.height}, Size: {GetBytesToString(sizeBytes)}</color>");
#endif

                request.Dispose();
                return t2d;
            }
            catch
            {
                request.Dispose();
                errorAction?.Invoke();
                Debug.Log($"<color=#FF0000>Request failed, URL: {url}</color>");
                return null;
            }
        }

        /// <summary>
        /// Sprite request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="position"></param>
        /// <param name="pivot"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public static async UniTask<Sprite> RequestSprite(string url, Action<Sprite> successAction = null, Action errorAction = null, Vector2 position = default, Vector2 pivot = default, float pixelPerUnit = 100, uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.FullRect, CancellationTokenSource cts = null, bool cached = true)
        {
            var texture = await RequestTexture2D(url, null, errorAction, cts, cached);
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
        /// <returns></returns>
        public static async UniTask<byte[]> RequestBytes(string url, Action<byte[]> successAction = null, Action errorAction = null, CancellationTokenSource cts = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.Log($"<color=#FF0000>Request failed, URL is null or empty.</color>");
                return new byte[] { };
            }

            UnityWebRequest request = null;
            try
            {
                request = UnityWebRequest.Get(url);

                if (cts != null) await request.SendWebRequest().WithCancellation(cts.Token);
                else await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.ConnectionError)
                {
                    request.Dispose();
                    errorAction?.Invoke();
                    Debug.Log($"<color=#FF0000>Request failed, URL: {url}</color>");
                    return new byte[] { };
                }

                byte[] bytes = request.downloadHandler.data;
                successAction?.Invoke(bytes);

#if UNITY_EDITOR
                ulong sizeBytes = (ulong)bytes.Length;
                Debug.Log($"<color=#90ff67>Request Bytes => Size: {GetBytesToString(sizeBytes)}</color>");
#endif

                request.Dispose();
                return bytes;
            }
            catch
            {
                request.Dispose();
                errorAction?.Invoke();
                Debug.Log($"<color=#FF0000>Request failed, URL: {url}</color>");
                return new byte[] { };
            }
        }

        /// <summary>
        /// File text request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="successAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public static async UniTask<string> RequestText(string url, Action<string> successAction = null, Action errorAction = null, CancellationTokenSource cts = null, bool cached = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.Log($"<color=#FF0000>Request failed, URL is null or empty.</color>");
                return null;
            }

            if (cached)
            {
                // ARCCache
                if (_arcTexts != null)
                {
                    string text = _arcTexts.Get(url);
                    if (text != null) return text;
                }
                // LRUCache
                else if (_lruTexts != null)
                {
                    string text = _lruTexts.Get(url);
                    if (text != null) return text;
                }
            }

            UnityWebRequest request = null;
            try
            {
                request = UnityWebRequest.Get(url);

                if (cts != null) await request.SendWebRequest().WithCancellation(cts.Token);
                else await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.ConnectionError)
                {
                    request.Dispose();
                    errorAction?.Invoke();
                    Debug.Log($"<color=#FF0000>Request failed, URL: {url}</color>");
                    return null;
                }

                string text = request.downloadHandler.text;
                if (cached)
                {
                    // ARCCache
                    if (_arcTexts != null)
                    {
                        _arcTexts.Add(url, text);
                        text = _arcTexts.Get(url);
                    }
                    // LRUCache
                    else if (_lruTexts != null)
                    {
                        _lruTexts.Add(url, text);
                        text = _lruTexts.Get(url);
                    }
                }
                successAction?.Invoke(text);

#if UNITY_EDITOR
                ulong sizeBytes = (ulong)request.downloadHandler.data.Length;
                Debug.Log($"<color=#90ff67>Request Text => Size: {GetBytesToString(sizeBytes)}</color>");
#endif

                request.Dispose();
                return text;
            }
            catch
            {
                request?.Dispose();
                errorAction?.Invoke();
                Debug.Log($"<color=#FF0000>Request failed, URL: {url}</color>");
                return null;
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
    }
}

using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.TestTools;

namespace OxGKit.Utilities.Tests
{
    public class RequesterTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [UnityTest]
        public IEnumerator RequestExistingAndNonExistingAudioUseARC()
        {
            // 初始緩存機制
            Requester.Requester.InitARCCacheCapacityForAudio();

            Stopwatch stopwatch = new Stopwatch();

            // 第一個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "audio.mp3").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Audio 1 (audio.mp3) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第二個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "audio.mp3").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Audio 2 (audio.mp3) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第三個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "null.mp3").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Audio 3 (null.mp3) Request Time: {stopwatch.ElapsedMilliseconds} ms");

            async UniTask Task(string baseUrl, string fileName)
            {
                var result = await Requester.Requester.RequestAudio($"{baseUrl}{fileName}", AudioType.MPEG, (result) =>
                {
                    Assert.IsNotNull(result);
                    UnityEngine.Debug.Log($"[InvokeSuccessCallback] Audio File: {fileName}, Length: {result.length}");
                },
                (error) =>
                {
                    Assert.IsNotNull(error.message);
                    UnityEngine.Debug.LogWarning($"[InvokeErrorCallback] {error.message}");
                });

                if (result != null)
                    Assert.IsNotNull(result);
                else
                    Assert.IsNull(result);
            }
        }

        [UnityTest]
        public IEnumerator RequestExistingAndNonExistingAudioUseLRU()
        {
            // 初始緩存機制
            Requester.Requester.InitLRUCacheCapacityForAudio();

            Stopwatch stopwatch = new Stopwatch();

            // 第一個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "audio.mp3").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Audio 1 (audio.mp3) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第二個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "audio.mp3").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Audio 2 (audio.mp3) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第三個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "null.mp3").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Audio 3 (null.mp3) Request Time: {stopwatch.ElapsedMilliseconds} ms");

            async UniTask Task(string baseUrl, string fileName)
            {
                var result = await Requester.Requester.RequestAudio($"{baseUrl}{fileName}", AudioType.MPEG, (result) =>
                {
                    Assert.IsNotNull(result);
                    UnityEngine.Debug.Log($"[InvokeSuccessCallback] Audio File: {fileName}, Length: {result.length}");
                },
                (error) =>
                {
                    Assert.IsNotNull(error.message);
                    UnityEngine.Debug.LogWarning($"[InvokeErrorCallback] {error.message}");
                });

                if (result != null)
                    Assert.IsNotNull(result);
                else
                    Assert.IsNull(result);
            }
        }

        [UnityTest]
        public IEnumerator RequestExistingAndNonExistingT2dUseARC()
        {
            // 初始緩存機制
            Requester.Requester.InitARCCacheCapacityForTexture2d();

            Stopwatch stopwatch = new Stopwatch();

            // 第一個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "t2d.png").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"T2d 1 (t2d.png) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第二個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "t2d.png").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"T2d 2 (t2d.png) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第三個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "null.png").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"T2d 3 (null.png) Request Time: {stopwatch.ElapsedMilliseconds} ms");

            async UniTask Task(string baseUrl, string fileName)
            {
                var result = await Requester.Requester.RequestTexture2D($"{baseUrl}{fileName}", (result) =>
                {
                    Assert.IsNotNull(result);
                    UnityEngine.Debug.Log($"[InvokeSuccessCallback] T2d File: {fileName}, Size: {result.GetRawTextureData().Length} (bytes)");
                },
                (error) =>
                {
                    Assert.IsNotNull(error.message);
                    UnityEngine.Debug.LogWarning($"[InvokeErrorCallback] {error.message}");
                });

                if (result != null)
                    Assert.IsNotNull(result);
                else
                    Assert.IsNull(result);
            }
        }

        [UnityTest]
        public IEnumerator RequestExistingAndNonExistingT2dUseLRU()
        {
            // 初始緩存機制
            Requester.Requester.InitLRUCacheCapacityForTexture2d();

            Stopwatch stopwatch = new Stopwatch();

            // 第一個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "t2d.png").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"T2d 1 (t2d.png) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第二個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "t2d.png").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"T2d 2 (t2d.png) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第三個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "null.png").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"T2d 3 (null.png) Request Time: {stopwatch.ElapsedMilliseconds} ms");

            async UniTask Task(string baseUrl, string fileName)
            {
                var result = await Requester.Requester.RequestTexture2D($"{baseUrl}{fileName}", (result) =>
                {
                    Assert.IsNotNull(result);
                    UnityEngine.Debug.Log($"[InvokeSuccessCallback] T2d File: {fileName}, Size: {result.GetRawTextureData().Length} (bytes)");
                },
                (error) =>
                {
                    Assert.IsNotNull(error.message);
                    UnityEngine.Debug.LogWarning($"[InvokeErrorCallback] {error.message}");
                });

                if (result != null)
                    Assert.IsNotNull(result);
                else
                    Assert.IsNull(result);
            }
        }

        [UnityTest]
        public IEnumerator RequestExistingAndNonExistingTextUseARC()
        {
            // 初始緩存機制
            Requester.Requester.InitARCCacheCapacityForText();

            Stopwatch stopwatch = new Stopwatch();

            // 第一個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "sheet.json").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Text 1 (sheet.json) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第二個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "sheet.json").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Text 2 (sheet.json) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第三個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "null.json").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Text 3 (null.json) Request Time: {stopwatch.ElapsedMilliseconds} ms");

            async UniTask Task(string baseUrl, string fileName)
            {
                var result = await Requester.Requester.RequestText($"{baseUrl}{fileName}", (result) =>
                {
                    Assert.IsNotNull(result);
                    UnityEngine.Debug.Log($"[InvokeSuccessCallback] Text File: {fileName}, Content: {result}");
                },
                (error) =>
                {
                    Assert.IsNotNull(error.message);
                    UnityEngine.Debug.LogWarning($"[InvokeErrorCallback] {error.message}");
                });

                if (result != null)
                    Assert.IsNotNull(result);
                else
                    Assert.IsNull(result);
            }
        }

        [UnityTest]
        public IEnumerator RequestExistingAndNonExistingTextUseLRU()
        {
            // 初始緩存機制
            Requester.Requester.InitLRUCacheCapacityForText();

            Stopwatch stopwatch = new Stopwatch();

            // 第一個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "sheet.json").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Text 1 (sheet.json) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第二個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "sheet.json").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Text 2 (sheet.json) Request Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // 第三個請求
            stopwatch.Start();
            yield return Task("http://127.0.0.1/", "null.json").ToCoroutine();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Text 3 (null.json) Request Time: {stopwatch.ElapsedMilliseconds} ms");

            async UniTask Task(string baseUrl, string fileName)
            {
                var result = await Requester.Requester.RequestText($"{baseUrl}{fileName}", (result) =>
                {
                    Assert.IsNotNull(result);
                    UnityEngine.Debug.Log($"[InvokeSuccessCallback] Text File: {fileName}, Content: {result}");
                },
                (error) =>
                {
                    Assert.IsNotNull(error.message);
                    UnityEngine.Debug.LogWarning($"[InvokeErrorCallback] {error.message}");
                });

                if (result != null)
                    Assert.IsNotNull(result);
                else
                    Assert.IsNull(result);
            }
        }
    }
}

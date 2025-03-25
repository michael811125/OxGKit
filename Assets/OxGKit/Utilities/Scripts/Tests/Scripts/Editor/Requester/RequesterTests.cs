using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace OxGKit.Utilities.Editor.Tests
{
    public class RequesterTests
    {
        [UnityTest]
        public IEnumerator RequestExistingAndNonExistingAudio()
        {
            yield return Task("http://127.0.0.1/", "audio.mp3").ToCoroutine();
            yield return Task("http://127.0.0.1/", "null.mp3").ToCoroutine();

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
        public IEnumerator RequestExistingAndNonExistingT2d()
        {
            yield return Task("http://127.0.0.1/", "t2d.png").ToCoroutine();
            yield return Task("http://127.0.0.1/", "null.png").ToCoroutine();

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
        public IEnumerator RequestExistingAndNonExistingSprite()
        {
            yield return Task("http://127.0.0.1/", "t2d.png").ToCoroutine();
            yield return Task("http://127.0.0.1/", "null.png").ToCoroutine();

            async UniTask Task(string baseUrl, string fileName)
            {
                var result = await Requester.Requester.RequestSprite($"{baseUrl}{fileName}", (result) =>
                {
                    Assert.IsNotNull(result);
                    UnityEngine.Debug.Log($"[InvokeSuccessCallback] T2d Sprite File: {fileName}, Size: {result.texture.GetRawTextureData().Length} (bytes)");
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
        public IEnumerator RequestExistingAndNonExistingText()
        {
            yield return Task("http://127.0.0.1/", "sheet.json").ToCoroutine();
            yield return Task("http://127.0.0.1/", "null.json").ToCoroutine();

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
        public IEnumerator RequestExistingAndNonExistingBytes()
        {
            yield return Task("http://127.0.0.1/", "sheet.bytes").ToCoroutine();
            yield return Task("http://127.0.0.1/", "null.bytes").ToCoroutine();

            async UniTask Task(string baseUrl, string fileName)
            {
                var result = await Requester.Requester.RequestBytes($"{baseUrl}{fileName}", (result) =>
                {
                    Assert.IsNotNull(result);
                    UnityEngine.Debug.Log($"[InvokeSuccessCallback] Bytes File: {fileName}, BufferSize: {result.Length}");
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

using NUnit.Framework;
using UnityEngine;

namespace OxGKit.SaverSystem.Editor.Tests
{
    public class SaverTests
    {
        private Saver _plyaerPrefSaver;
        private Saver _editorPrefSaver;

        private const string _CONTENT_KEY = "_CONTENT_KEY";

        [SetUp]
        public void Setup()
        {
            this._plyaerPrefSaver = new PlayerPrefsSaver();
            this._editorPrefSaver = new EditorPrefsSaver();
        }

        #region PlayerPrefs
        [Test]
        public void PlayerPrefsSaveAndGetData()
        {
            for (int i = 0; i < 10; i++)
                this._plyaerPrefSaver.SaveData(_CONTENT_KEY, $"{i}_key", $"{i}_value");

            Debug.Log(this._plyaerPrefSaver.GetString(_CONTENT_KEY));

            for (int i = 0; i < 10; i++)
                Debug.Log(this._plyaerPrefSaver.GetData(_CONTENT_KEY, $"{i}_key", string.Empty));
        }

        [Test]
        public void PlayerPrefsGetData()
        {
            for (int i = 0; i < 10; i++)
                Debug.Log(this._plyaerPrefSaver.GetData(_CONTENT_KEY, $"{i}_key", string.Empty));
        }

        [Test]
        public void PlayerPrefsDeleteAndGetData()
        {
            for (int i = 5; i < 10; i++)
                this._plyaerPrefSaver.DeleteData(_CONTENT_KEY, $"{i}_key");

            for (int i = 0; i < 10; i++)
                Debug.Log(this._plyaerPrefSaver.GetData(_CONTENT_KEY, $"{i}_key", string.Empty));
        }

        [Test]
        public void PlayerPrefsDeleteContextAndGetData()
        {
            this._plyaerPrefSaver.DeleteContext(_CONTENT_KEY);

            for (int i = 0; i < 10; i++)
                Debug.Log(this._plyaerPrefSaver.GetData(_CONTENT_KEY, $"{i}_key", string.Empty));
        }
        #endregion

        #region EditorPrefs
        [Test]
        public void EditorPrefsSaveAndGetData()
        {
            for (int i = 0; i < 10; i++)
                this._editorPrefSaver.SaveData(_CONTENT_KEY, $"{i}_key", $"{i}_value");

            Debug.Log(this._editorPrefSaver.GetString(_CONTENT_KEY));

            for (int i = 0; i < 10; i++)
                Debug.Log(this._editorPrefSaver.GetData(_CONTENT_KEY, $"{i}_key", string.Empty));
        }

        [Test]
        public void EditorPrefsGetData()
        {
            for (int i = 0; i < 10; i++)
                Debug.Log(this._editorPrefSaver.GetData(_CONTENT_KEY, $"{i}_key", string.Empty));
        }

        [Test]
        public void EditorPrefsDeleteAndGetData()
        {
            for (int i = 5; i < 10; i++)
                this._editorPrefSaver.DeleteData(_CONTENT_KEY, $"{i}_key");

            for (int i = 0; i < 10; i++)
                Debug.Log(this._editorPrefSaver.GetData(_CONTENT_KEY, $"{i}_key", string.Empty));
        }

        [Test]
        public void EditorPrefsDeleteContextAndGetData()
        {
            this._editorPrefSaver.DeleteContext(_CONTENT_KEY);

            for (int i = 0; i < 10; i++)
                Debug.Log(this._editorPrefSaver.GetData(_CONTENT_KEY, $"{i}_key", string.Empty));
        }
        #endregion
    }
}

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace OxGKit.Utilities.Tests
{
    public class DontDestroyTests
    {
        private GameObject _testObject;
        private DontDestroy.DontDestroy _testComponent;

        [SetUp]
        public void SetUp()
        {
            this._testObject = new GameObject();
            this._testComponent = this._testObject.AddComponent<DontDestroy.DontDestroy>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(this._testObject);
        }

        [UnityTest]
        public IEnumerator DontDestroySetter()
        {
            string goName = nameof(DontDestroyTests);
            this._testComponent.SetRuntimeName(goName);

            yield return new WaitForSeconds(3);
        }
    }
}

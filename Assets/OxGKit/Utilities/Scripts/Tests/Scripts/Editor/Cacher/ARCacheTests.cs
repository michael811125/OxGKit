using NUnit.Framework;
using OxGKit.Utilities.Cacher;

namespace OxGKit.Utilities.Editor.Tests
{
    public class ARCCacheTests
    {
        private ARCCache<int, string> _cache;

        [SetUp]
        public void Setup()
        {
            this._cache = new ARCCache<int, string>(3);
        }

        [Test]
        public void AddAndGetItem()
        {
            this._cache.Add(1, "one");
            Assert.IsTrue(this._cache.Contains(1));
            Assert.AreEqual("one", this._cache.Get(1));
        }

        [Test]
        public void GetNonExistentItemReturnsDefault()
        {
            Assert.AreEqual(default(string), this._cache.Get(99));
        }

        [Test]
        public void AddDuplicateKeyUpdatesValue()
        {
            this._cache.Add(1, "one");
            this._cache.Add(1, "uno");
            this._cache.Add(1, "foo");
            Assert.AreEqual("foo", this._cache.Get(1));
        }

        [Test]
        public void CacheEvictsItemsUsingARCPolicy()
        {
            this._cache.Add(1, "one");
            this._cache.Add(2, "two");
            this._cache.Add(3, "three");

            // Access key 1 to move it to the front
            this._cache.Get(1);

            // Add new item, should evict least used (2)
            this._cache.Add(4, "four");

            Assert.IsTrue(this._cache.Contains(1));
            Assert.IsTrue(this._cache.Contains(3));
            Assert.IsTrue(this._cache.Contains(4));
            Assert.IsFalse(this._cache.Contains(2));
        }

        [Test]
        public void RemoveItem()
        {
            this._cache.Add(1, "one");
            this._cache.Add(2, "two");

            Assert.IsTrue(this._cache.Remove(1), "Should successfully remove existing item");
            Assert.IsFalse(this._cache.Contains(1), "Removed item should not be in cache");
        }

        [Test]
        public void ClearCache()
        {
            this._cache.Add(1, "one");
            this._cache.Add(2, "two");
            this._cache.Clear();

            Assert.AreEqual(0, this._cache.Count, "Cache should be empty after clear");
        }

        [Test]
        public void AddNullKeyDoesNotThrowException()
        {
            Assert.DoesNotThrow(() => this._cache.Add(default, "test"));
        }

        [Test]
        public void GetKeysReturnsCorrectKeys()
        {
            this._cache.Add(1, "one");
            this._cache.Add(2, "two");

            var keys = this._cache.GetKeys();

            Assert.AreEqual(2, keys.Length, "Should return correct number of keys");
            Assert.Contains(1, keys);
            Assert.Contains(2, keys);
        }
    }
}
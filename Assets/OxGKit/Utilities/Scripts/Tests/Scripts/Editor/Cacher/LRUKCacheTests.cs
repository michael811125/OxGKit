using NUnit.Framework;
using OxGKit.Utilities.Cacher;

namespace OxGKit.Utilities.Editor.Tests
{
    public class LRUKCacheTests
    {
        private LRUKCache<int, string> _cache;

        [SetUp]
        public void Setup()
        {
            this._cache = new LRUKCache<int, string>(3, 2);
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
            Assert.AreEqual("uno", this._cache.Get(1));
        }

        [Test]
        public void CacheEvictsLessFrequentlyUsedItem()
        {
            this._cache.Add(1, "one");
            this._cache.Add(2, "two");
            this._cache.Add(3, "three");

            // Access key 1 twice to increase its counter
            this._cache.Get(1);
            this._cache.Get(1);

            // Add new item, should evict least used (2)
            this._cache.Add(4, "four");

            Assert.IsTrue(this._cache.Contains(1));
            Assert.IsTrue(this._cache.Contains(4));
            Assert.IsTrue(this._cache.Contains(3));
            Assert.IsFalse(this._cache.Contains(2));
            Assert.AreEqual(3, this._cache.Count);
        }

        [Test]
        public void RemoveItem()
        {
            this._cache.Add(1, "one");
            this._cache.Add(2, "two");
            Assert.IsTrue(this._cache.Remove(1));
            Assert.IsFalse(this._cache.Contains(1));
        }

        [Test]
        public void ClearCache()
        {
            this._cache.Add(1, "one");
            this._cache.Add(2, "two");
            this._cache.Clear();
            Assert.AreEqual(0, this._cache.Count);
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
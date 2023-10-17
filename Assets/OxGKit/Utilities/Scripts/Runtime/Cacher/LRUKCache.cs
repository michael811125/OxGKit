using System.Collections.Generic;
using System.Linq;

namespace OxGKit.Utilities.Cacher
{
    public class LRUKCache<TKey, TValue>
    {
        private readonly int _capacity;
        private readonly int _k;
        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cache;
        private readonly LinkedList<CacheItem> _lruList;

        public int Count => this._cache.Count;

        public LRUKCache(int capacity, int k)
        {
            this._capacity = capacity;
            this._k = k;
            this._cache = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
            this._lruList = new LinkedList<CacheItem>();
        }

        public TKey[] GetKeys()
        {
            return this._cache.Keys.ToArray();
        }

        public bool Contains(TKey key)
        {
            return this._cache.ContainsKey(key);
        }

        public TValue Get(TKey key)
        {
            if (this._cache.TryGetValue(key, out var node))
            {
                this.IncrementCounter(node.Value.Counter);
                this._lruList.Remove(node);
                this._lruList.AddLast(node);
                return node.Value.Value;
            }
            return default;
        }

        public void Add(TKey key, TValue value)
        {
            if (this._cache.Count >= this._capacity && !this._cache.ContainsKey(key))
            {
                this.Evict();
            }

            if (this._cache.TryGetValue(key, out var node))
            {
                node.Value.Value = value;
                this.IncrementCounter(node.Value.Counter);
                this._lruList.Remove(node);
                this._lruList.AddLast(node);
            }
            else
            {
                var newNode = new LinkedListNode<CacheItem>(new CacheItem(key, value));
                this._cache.Add(key, newNode);
                this.IncrementCounter(newNode.Value.Counter);
                this._lruList.AddLast(newNode);
            }
        }

        public bool Remove(TKey key)
        {
            if (this._cache.TryGetValue(key, out var node))
            {
                // For Unity
                var item = node.Value.Value;
                if (item is UnityEngine.Object) UnityEngine.Object.Destroy(item as UnityEngine.Object);
                node.Value.Value = default;
                this._lruList.Remove(node);
                this._cache.Remove(key);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            var keys = this.GetKeys();
            foreach (var key in keys)
            {
                this.Remove(key);
            }
            this._lruList.Clear();
            this._cache.Clear();
        }

        protected void IncrementCounter(int counter)
        {
            foreach (var node in this._cache.Values)
            {
                if (node.Value.Counter < _k || node.Value.Counter == counter)
                {
                    node.Value.Counter++;
                }
            }
        }

        protected void DecrementCounters()
        {
            foreach (var node in this._cache.Values)
            {
                node.Value.Counter--;
            }
        }

        protected int FindMinCounter()
        {
            var minCounter = int.MaxValue;

            foreach (var node in this._cache.Values)
            {
                if (node.Value.Counter < minCounter)
                {
                    minCounter = node.Value.Counter;
                }
            }

            return minCounter;
        }

        protected void Evict()
        {
            var minCounter = this.FindMinCounter();
            var node = this._lruList.First;

            while (node != null)
            {
                if (node.Value.Counter <= this._k && node.Value.Counter == minCounter)
                {
                    // For Unity
                    var item = node.Value.Value;
                    if (item is UnityEngine.Object) UnityEngine.Object.Destroy(item as UnityEngine.Object);
                    node.Value.Value = default;
                    this._cache.Remove(node.Value.Key);
                    this.DecrementCounters();
                    this._lruList.Remove(node);
                    break;
                }
                node = node.Next;
            }
        }

        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; set; }
            public int Counter { get; set; }

            public CacheItem(TKey key, TValue value)
            {
                this.Key = key;
                this.Value = value;
                this.Counter = 0;
            }
        }
    }
}

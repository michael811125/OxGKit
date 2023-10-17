using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OxGKit.Utilities.Cacher
{
    public class LRUCache<TKey, TValue>
    {
        private readonly int _capacity;
        private readonly ConcurrentDictionary<TKey, LinkedListNode<CacheItem>> _cache;
        private readonly LinkedList<CacheItem> _lruList;

        public int Count => this._cache.Count;

        public LRUCache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.");

            this._capacity = capacity;
            this._cache = new ConcurrentDictionary<TKey, LinkedListNode<CacheItem>>();
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
                // Move the accessed item to the front of the LRU list
                this._lruList.Remove(node);
                this._lruList.AddFirst(node);

                return node.Value.Value;
            }

            return default;
        }

        public void Add(TKey key, TValue value)
        {
            if (this._cache.Count >= this._capacity)
                this.RemoveLRUItem();

            var cacheItem = new CacheItem(key, value);
            var newNode = new LinkedListNode<CacheItem>(cacheItem);
            this._lruList.AddFirst(newNode);
            this._cache.TryAdd(key, newNode);
        }

        public bool Remove(TKey key)
        {
            if (this._cache.TryGetValue(key, out var node))
            {
                // For Unity
                var item = node.Value.Value;
                if (item is UnityEngine.AudioClip &&
                    item != null) UnityEngine.Object.Destroy(item as UnityEngine.AudioClip);
                else if (item is UnityEngine.Texture2D &&
                    item != null) UnityEngine.Object.Destroy(item as UnityEngine.Texture2D);
                node.Value.Value = default;
                this._lruList.Remove(node);
                this._cache.Remove(key, out _);
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

        protected void RemoveLRUItem()
        {
            var lastNode = this._lruList.Last;
            // For Unity
            var item = lastNode.Value.Value;
            if (item != null &&
                item is UnityEngine.AudioClip) UnityEngine.Object.Destroy(item as UnityEngine.AudioClip);
            else if (item != null &&
                item is UnityEngine.Texture2D) UnityEngine.Object.Destroy(item as UnityEngine.Texture2D);
            lastNode.Value.Value = default;
            this._cache.TryRemove(lastNode.Value.Key, out _);
            this._lruList.RemoveLast();
        }

        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; set; }

            public CacheItem(TKey key, TValue value)
            {
                this.Key = key;
                this.Value = value;
            }
        }
    }
}
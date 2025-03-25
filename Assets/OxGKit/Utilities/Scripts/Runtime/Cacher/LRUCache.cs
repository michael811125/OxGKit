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
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 特殊處理
        /// </summary>
        private IRemoveCacheHandler<TKey, TValue> _removeCacheHandler;

        public int Count
        {
            get
            {
                lock (this._syncRoot)
                {
                    return this._cache?.Count ?? 0;
                }
            }
        }

        public LRUCache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.");

            this._capacity = capacity;
            this._cache = new ConcurrentDictionary<TKey, LinkedListNode<CacheItem>>();
            this._lruList = new LinkedList<CacheItem>();
            this._removeCacheHandler = new UnityObjectRemoveCacheHandler<TKey, TValue>();
        }

        public LRUCache(int capacity, IRemoveCacheHandler<TKey, TValue> removeCacheHandler) : this(capacity)
        {
            this._removeCacheHandler = removeCacheHandler;
        }

        public TKey[] GetKeys()
        {
            lock (this._syncRoot)
            {
                return this._cache.Keys.ToArray();
            }
        }

        public bool Contains(TKey key)
        {
            lock (this._syncRoot)
            {
                return this._cache.ContainsKey(key);
            }
        }

        public TValue Get(TKey key)
        {
            lock (this._syncRoot)
            {
                if (this._cache.TryGetValue(key, out var node))
                {
                    // 如果節點不在最前面, 則移動
                    if (this._lruList.First != node)
                    {
                        this._lruList.Remove(node);
                        this._lruList.AddFirst(node);
                    }
                    return node.Value.Value;
                }
            }
            return default;
        }

        public void Add(TKey key, TValue value)
        {
            lock (this._syncRoot)
            {
                // 如果已經存在, 則更新值並移動
                if (this._cache.TryGetValue(key, out var existingNode))
                {
                    existingNode.Value.Value = value;

                    // 移動到最前面
                    this._lruList.Remove(existingNode);
                    this._lruList.AddFirst(existingNode);
                }
                else
                {
                    // 緩存滿時淘汰最久未使用的項目
                    if (this._cache.Count >= this._capacity)
                    {
                        this.RemoveLRUItem();
                    }

                    var cacheItem = new CacheItem(key, value);
                    var newNode = new LinkedListNode<CacheItem>(cacheItem);

                    // 將新節點加到最前面
                    this._lruList.AddFirst(newNode);
                    this._cache.TryAdd(key, newNode);
                }
            }
        }

        public bool Remove(TKey key)
        {
            lock (this._syncRoot)
            {
                if (this._cache.TryRemove(key, out var node))
                {
                    // For remove handler
                    var item = node.Value.Value;
                    this._removeCacheHandler?.RemoveCache(key, item);
                    node.Value.Value = default;
                    // 從 LRU 列表中移除
                    this._lruList.Remove(node);
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            lock (this._syncRoot)
            {
                var keys = this.GetKeys();
                foreach (var key in keys)
                {
                    this.Remove(key);
                }
                this._lruList.Clear();
                this._cache.Clear();
            }
        }

        protected void RemoveLRUItem()
        {
            lock (this._syncRoot)
            {
                var lastNode = this._lruList.Last;
                if (lastNode != null)
                {
                    // For remove handler
                    var key = lastNode.Value.Key;
                    var item = lastNode.Value.Value;
                    this._removeCacheHandler?.RemoveCache(key, item);
                    lastNode.Value.Value = default;
                    this._cache.TryRemove(key, out _);
                    this._lruList.RemoveLast();
                }
            }
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
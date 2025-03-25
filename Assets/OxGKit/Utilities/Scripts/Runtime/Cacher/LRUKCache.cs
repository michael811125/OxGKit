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
        private readonly object _syncRoot = new object();

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

        public LRUKCache(int capacity, int k)
        {
            this._capacity = capacity;
            this._k = k;
            this._cache = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
            this._lruList = new LinkedList<CacheItem>();
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
                    // 對當前節點更新 Counter
                    if (node.Value.Counter < this._k)
                    {
                        node.Value.Counter++;
                    }
                    // 將節點移到最新使用 (LRU 列表尾部)
                    this._MoveToEndOfLRU(node);
                    return node.Value.Value;
                }
                return default;
            }
        }

        public void Add(TKey key, TValue value)
        {
            lock (this._syncRoot)
            {
                // 當緩存滿且新增的是新鍵時進行淘汰
                if (this._cache.Count >= this._capacity && !this._cache.ContainsKey(key))
                {
                    this.Evict();
                }

                if (this._cache.TryGetValue(key, out var node))
                {
                    node.Value.Value = value;
                    if (node.Value.Counter < this._k)
                    {
                        node.Value.Counter++;
                    }
                    this._MoveToEndOfLRU(node);
                }
                else
                {
                    var newNode = new LinkedListNode<CacheItem>(new CacheItem(key, value));
                    // 初始時將 Counter 設為 1
                    newNode.Value.Counter = 1;
                    this._cache.Add(key, newNode);
                    this._lruList.AddLast(newNode);
                }
            }
        }

        public bool Remove(TKey key)
        {
            lock (this._syncRoot)
            {
                if (this._cache.TryGetValue(key, out var node))
                {
                    // For Unity
                    var item = node.Value.Value;
                    if (item is UnityEngine.Object)
                        UnityEngine.Object.Destroy(item as UnityEngine.Object);
                    this._lruList.Remove(node);
                    this._cache.Remove(key);
                    return true;
                }
                return false;
            }
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

        /// <summary>
        /// 將節點移動到 LRU 列表的尾部 (最新使用)
        /// </summary>
        private void _MoveToEndOfLRU(LinkedListNode<CacheItem> node)
        {
            this._lruList.Remove(node);
            this._lruList.AddLast(node);
        }

        /// <summary>
        /// 淘汰一個最久未使用且人氣最低的項目
        /// </summary>
        protected void Evict()
        {
            lock (this._syncRoot)
            {
                // 從 LRU 列表首端開始尋找淘汰項目
                var node = this._lruList.First;
                int minCounter = this.FindMinCounter();
                while (node != null)
                {
                    if (node.Value.Counter <= this._k && node.Value.Counter == minCounter)
                    {
                        var item = node.Value.Value;
                        if (item is UnityEngine.Object)
                            UnityEngine.Object.Destroy(item as UnityEngine.Object);
                        this._cache.Remove(node.Value.Key);
                        // 淘汰時對其餘項目進行衰減, 使人氣動態調整
                        this.DecrementCounters();
                        this._lruList.Remove(node);
                        break;
                    }
                    node = node.Next;
                }
            }
        }

        /// <summary>
        /// 尋找目前所有項目的最小 Counter
        /// </summary>
        protected int FindMinCounter()
        {
            lock (this._syncRoot)
            {
                int minCounter = int.MaxValue;
                foreach (var node in this._cache.Values)
                {
                    if (node.Value.Counter < minCounter)
                    {
                        minCounter = node.Value.Counter;
                    }
                }
                return minCounter;
            }
        }

        /// <summary>
        /// 淘汰後對所有項目進行衰減
        /// </summary>
        protected void DecrementCounters()
        {
            lock (this._syncRoot)
            {
                foreach (var node in this._cache.Values)
                {
                    if (node.Value.Counter > 0)
                        node.Value.Counter--;
                }
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
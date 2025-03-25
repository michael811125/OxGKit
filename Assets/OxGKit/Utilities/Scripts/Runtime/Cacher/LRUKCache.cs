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
        private SortedSet<(int counter, TKey key)> _minHeap = new();
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

        public LRUKCache(int capacity, int k)
        {
            this._capacity = capacity;
            this._k = k;
            this._cache = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
            this._lruList = new LinkedList<CacheItem>();
            this._removeCacheHandler = new UnityObjectRemoveCacheHandler<TKey, TValue>();
        }

        public LRUKCache(int capacity, int k, IRemoveCacheHandler<TKey, TValue> removeCacheHandler) : this(capacity, k)
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
                    int oldCounter = node.Value.Counter;
                    if (node.Value.Counter < this._k)
                    {
                        node.Value.Counter++;
                    }
                    int newCounter = node.Value.Counter;

                    // 更新 minHeap
                    this.UpdateMinHeap(key, oldCounter, newCounter);

                    // 移動到 LRU 列表尾部
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
                if (this._cache.Count >= this._capacity && !this._cache.ContainsKey(key))
                {
                    this.Evict();
                }

                if (this._cache.TryGetValue(key, out var node))
                {
                    int oldCounter = node.Value.Counter;
                    node.Value.Value = value;
                    if (node.Value.Counter < this._k)
                    {
                        node.Value.Counter++;
                    }
                    int newCounter = node.Value.Counter;

                    // 更新 minHeap
                    this.UpdateMinHeap(key, oldCounter, newCounter);

                    this._MoveToEndOfLRU(node);
                }
                else
                {
                    var newNode = new LinkedListNode<CacheItem>(new CacheItem(key, value));
                    newNode.Value.Counter = 1;
                    this._cache.Add(key, newNode);
                    this._lruList.AddLast(newNode);

                    // 新增到 minHeap
                    this.UpdateMinHeap(key, 0, 1);
                }
            }
        }

        public bool Remove(TKey key)
        {
            lock (this._syncRoot)
            {
                if (this._cache.TryGetValue(key, out var node))
                {
                    // For remove handler
                    var item = node.Value.Value;
                    this._removeCacheHandler?.RemoveCache(key, item);
                    this._lruList.Remove(node);
                    this._cache.Remove(key);

                    // 確保從 minHeap 內移除
                    this._minHeap.Remove((node.Value.Counter, key));

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
                var node = this._lruList.First;
                int minCounter = this.FindMinCounter();

                while (node != null)
                {
                    if (node.Value.Counter <= this._k && node.Value.Counter == minCounter)
                    {
                        var key = node.Value.Key;
                        var item = node.Value.Value;
                        this._removeCacheHandler?.RemoveCache(key, item);
                        this._cache.Remove(key);

                        // 確保從 minHeap 內也移除
                        this._minHeap.Remove((node.Value.Counter, key));

                        // 淘汰時對其餘項目進行衰減
                        this.DecrementCounters();
                        this._lruList.Remove(node);
                        break;
                    }
                    node = node.Next;
                }
            }
        }


        /// <summary>
        /// 更新 Counter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="oldCounter"></param>
        /// <param name="newCounter"></param>
        protected void UpdateMinHeap(TKey key, int oldCounter, int newCounter)
        {
            lock (this._syncRoot)
            {
                this._minHeap.Remove((oldCounter, key));
                this._minHeap.Add((newCounter, key));
            }
        }

        /// <summary>
        /// 尋找目前所有項目的最小 Counter
        /// </summary>
        protected int FindMinCounter()
        {
            lock (this._syncRoot)
            {
                return this._minHeap.Count > 0 ? this._minHeap.Min.counter : int.MaxValue;
            }
        }

        /// <summary>
        /// 淘汰後對所有項目進行衰減
        /// </summary>
        protected void DecrementCounters()
        {
            lock (this._syncRoot)
            {
                var newMinHeap = new SortedSet<(int counter, TKey key)>();

                foreach (var node in this._cache.Values)
                {
                    if (node.Value.Counter > 0)
                    {
                        node.Value.Counter--;
                    }

                    // 直接重新建立 minHeap
                    newMinHeap.Add((node.Value.Counter, node.Value.Key));
                }

                this._minHeap = newMinHeap;
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
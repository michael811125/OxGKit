using System;
using System.Collections.Generic;
using System.Linq;

namespace OxGKit.Utilities.Cacher
{
    public class ARCCache<TKey, TValue>
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly LinkedList<TKey> _t1;
        private readonly LinkedList<TKey> _t2;
        private readonly HashSet<TKey> _b1b2;
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

        public ARCCache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.");

            this._capacity = capacity;
            this._cache = new Dictionary<TKey, TValue>(capacity);
            this._t1 = new LinkedList<TKey>();
            this._t2 = new LinkedList<TKey>();
            this._b1b2 = new HashSet<TKey>();
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
                if (this._cache.TryGetValue(key, out var value))
                {
                    this.MoveToT2(key);
                    return value;
                }
                return default;
            }
        }

        public void Add(TKey key, TValue value)
        {
            lock (this._syncRoot)
            {
                if (this._cache.ContainsKey(key))
                {
                    this._cache[key] = value;
                    this.MoveToT2(key);
                }
                else
                {
                    if (this._cache.Count >= this._capacity)
                        this.PerformEviction();

                    this._cache.Add(key, value);
                    this.MoveToT1(key);
                }
            }
        }

        public bool Remove(TKey key)
        {
            lock (this._syncRoot)
            {
                if (this._cache.ContainsKey(key))
                {
                    this._t1.Remove(key);
                    this._t2.Remove(key);
                    this._b1b2.Remove(key);
                    this._RemoveCacheEntry(key);
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
                this._t1.Clear();
                this._t2.Clear();
                this._b1b2.Clear();
                this._cache.Clear();
            }
        }

        protected void PerformEviction()
        {
            lock (this._syncRoot)
            {
                if (this._t1.Count > 0)
                {
                    var keyToRemove = this._t1.Last.Value;
                    this._t1.RemoveLast();
                    this._b1b2.Remove(keyToRemove);
                    this._RemoveCacheEntry(keyToRemove);
                }
                else if (this._t2.Count > 0)
                {
                    var keyToRemove = this._t2.Last.Value;
                    this._t2.RemoveLast();
                    this._b1b2.Remove(keyToRemove);
                    this._RemoveCacheEntry(keyToRemove);
                }
            }
        }

        private void _RemoveCacheEntry(TKey key)
        {
            // For Unity
            var item = this._cache[key];
            if (item is UnityEngine.Object)
                UnityEngine.Object.Destroy(item as UnityEngine.Object);
            this._cache.Remove(key);
        }

        protected void MoveToT1(TKey key)
        {
            lock (this._syncRoot)
            {
                if (this._t1.Contains(key))
                    this._t1.Remove(key);
                else if (this._t2.Contains(key))
                {
                    this._t2.Remove(key);
                    if (this._t1.Count >= this._capacity && this._t1.Count > 0)
                        this._b1b2.Remove(this._t1.Last.Value);
                }
                else
                {
                    if (this._cache.Count >= this._capacity && this._t2.Count > 0)
                        this._b1b2.Remove(this._t2.Last.Value);
                }

                this._t1.AddFirst(key);
            }
        }

        protected void MoveToT2(TKey key)
        {
            lock (this._syncRoot)
            {
                if (this._t2.Contains(key))
                {
                    this._t2.Remove(key);
                }
                else if (this._t1.Contains(key))
                {
                    this._t1.Remove(key);
                    if (this._t2.Count >= this._capacity && this._t2.Count > 0)
                        this._b1b2.Remove(this._t2.Last.Value);
                }
                else
                {
                    if (this._cache.Count >= this._capacity && this._t1.Count > 0)
                        this._b1b2.Remove(this._t1.Last.Value);
                }

                this._t2.AddFirst(key);
            }
        }
    }
}
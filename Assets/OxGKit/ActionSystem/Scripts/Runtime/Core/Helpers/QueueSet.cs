using System.Collections.Generic;
using System.Linq;

namespace OxGKit.ActionSystem
{
    public class QueueSet<T>
    {
        private HashSet<T> _set;
        private Queue<T> _queue;

        public QueueSet()
        {
            this._set = new HashSet<T>();
            this._queue = new Queue<T>();
        }

        public void Enqueue(T item)
        {
            if (!this._set.Contains(item))
            {
                this._set.Add(item);
                this._queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            T item = this._queue.Dequeue();
            this._set.Remove(item);
            return item;
        }

        public bool Remove(T item)
        {
            if (this._set.Contains(item))
            {
                this._set.Remove(item);
                this._queue = new Queue<T>(this._queue.Where(x => !EqualityComparer<T>.Default.Equals(x, item)));
                return true;
            }
            return false;
        }

        public void Clear()
        {
            this._set.Clear();
            this._queue.Clear();
        }

        public bool Contains(T item)
        {
            return this._set.Contains(item);
        }

        public int Count
        {
            get { return this._queue.Count; }
        }

        public T[] ToArray()
        {
            return this._queue.ToArray();
        }
    }
}
using System;
using System.Collections.Generic;

namespace OxGKit.NoticeSystem
{
    public class ListSet<T>
    {
        private HashSet<T> _set;
        private List<T> _list;

        public ListSet()
        {
            this._set = new HashSet<T>();
            this._list = new List<T>();
        }

        public void Add(T item)
        {
            if (!this._set.Contains(item))
            {
                this._set.Add(item);
                this._list.Add(item);
            }
        }

        public void Assign(T item)
        {
            if (this._set.Contains(item))
            {
                int index = this._list.IndexOf(item);
                this.RemoveAt(index);
                this.Add(item);
            }
        }

        public T RemoveAt(int index)
        {
            T item = this._list[index];
            this._set.Remove(item);
            this._list.RemoveAt(index);
            return item;
        }

        public bool Remove(T item)
        {
            if (this._set.Contains(item))
            {
                this._set.Remove(item);
                this._list.Remove(item);
                return true;
            }
            return false;
        }

        public bool Contains(T item)
        {
            return this._set.Contains(item);
        }

        public int Count
        {
            get { return this._list.Count; }
        }

        public T[] ToArray()
        {
            return this._list.ToArray();
        }

        public List<T> GetList()
        {
            return this._list;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OxGKit.Utilities.UMT
{
    [DisallowMultipleComponent]
    internal class UnityMainThread : MonoBehaviour
    {
        internal static UnityMainThread worker;
        internal static readonly object threadLocker = new object();
        private Queue<Action> _jobs = new Queue<Action>();

        private void Awake()
        {
            worker = this;

            string newName = $"[{nameof(UnityMainThread)}]";
            this.gameObject.name = newName;
            if (this.gameObject.transform.root.name == newName)
            {
                var container = GameObject.Find(nameof(OxGKit));
                if (container == null) container = new GameObject(nameof(OxGKit));
                this.gameObject.transform.SetParent(container.transform);
                DontDestroyOnLoad(container);
            }
            else DontDestroyOnLoad(this.gameObject.transform.root);
        }

        private void Update()
        {
            while (this._jobs.Count > 0)
            {
                lock (threadLocker)
                {
                    this._jobs.Dequeue()?.Invoke();
                }
            }
        }

        internal void AddJob(Action newJob)
        {
            lock (threadLocker)
            {
                this._jobs.Enqueue(newJob);
            }
        }

        public void Clear()
        {
            this._jobs.Clear();
        }
    }
}


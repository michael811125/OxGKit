using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OxGKit.Utilities.UnityMainThread
{
    [DisallowMultipleComponent]
    public class UMT : MonoBehaviour
    {
        public static UMT worker => GetInstance();
        internal static readonly object threadLocker = new object();
        private Queue<Action> _jobs = new Queue<Action>();

        private static readonly object _locker = new object();
        private static UMT _instance = null;
        internal static UMT GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    _instance = FindObjectOfType<UMT>();
                    if (_instance == null)
                        _instance = new GameObject(nameof(UMT)).AddComponent<UMT>();
                }
            }
            return _instance;
        }

        private void Awake()
        {
            string newName = $"[{nameof(UMT)}]";
            this.gameObject.name = newName;
            if (this.gameObject.transform.root.name == newName)
            {
                var container = GameObject.Find(nameof(OxGKit));
                if (container == null)
                    container = new GameObject(nameof(OxGKit));
                this.gameObject.transform.SetParent(container.transform);
                DontDestroyOnLoad(container);
            }
            else
                DontDestroyOnLoad(this.gameObject.transform.root);
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

        public void AddJob(Action newJob)
        {
            lock (threadLocker)
            {
                this._jobs.Enqueue(newJob);
            }
        }

        public void RunCoroutine(IEnumerator routine)
        {
            this.StartCoroutine(routine);
        }

        public void RunCoroutine(string methodName)
        {
            this.StartCoroutine(methodName);
        }

        public void CancelCoroutine(IEnumerator routine)
        {
            this.StopCoroutine(routine);
        }

        public void CancelCoroutine(string methodName)
        {
            this.StopCoroutine(methodName);
        }

        public void CancelAllCoroutine()
        {
            this.StopAllCoroutines();
        }

        public void Clear()
        {
            this._jobs.Clear();
        }
    }
}
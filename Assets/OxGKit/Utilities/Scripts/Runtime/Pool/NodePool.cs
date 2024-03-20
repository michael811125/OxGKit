using System.Collections.Generic;
using UnityEngine;
using MyBox;

namespace OxGKit.Utilities.Pool
{
    [AddComponentMenu("OxGKit/Utilities/Pool/" + nameof(NodePool))]
    public class NodePool : MonoBehaviour
    {
        [SerializeField, Tooltip("Pool object")]
        public GameObject go = null;
        [SerializeField, Tooltip("Pool initial size")]
        public int initSize = 0;
        [SerializeField, Tooltip("If checked, when pool not enough will auto create")]
        public bool autoPut = false;
        [SerializeField, ConditionalField(nameof(autoPut)), Tooltip("Each increment count when auto create")]
        public int autoPutSize = 0;
        private Queue<GameObject> _pool;

        private void Awake()
        {
            this._pool = new Queue<GameObject>();
        }

        private void Start()
        {
            this._Init();
        }

        private void _Init()
        {
            for (int i = 0; i < this.initSize; i++)
            {
                GameObject instGo = Instantiate(this.go, Vector3.zero, Quaternion.identity, this.transform);
                this.Put(instGo);
            }
        }

        private void _AutoPut()
        {
            for (int i = 0; i < this.autoPutSize; i++)
            {
                GameObject instGo = Instantiate(this.go, Vector3.zero, Quaternion.identity, this.transform);
                this.Put(instGo);
            }
        }

        /// <summary>
        /// Pool size
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            return this._pool.Count;
        }

        /// <summary>
        /// Clear pool
        /// </summary>
        public void Clear()
        {
            foreach (var go in this._pool) Destroy(go);
            this._pool.Clear();
        }

        /// <summary>
        /// Recycle GameObject
        /// </summary>
        /// <param name="go"></param>
        public void Put(GameObject go)
        {
            if (go)
            {
                go.transform.SetParent(this.transform);
                go.SetActive(false);
                this._pool.Enqueue(go);
            }
        }

        /// <summary>
        /// Get GameObject from pool
        /// </summary>
        /// <returns></returns>
        public GameObject Get()
        {
            if (this._pool.Count == 0)
            {
                if (this.autoPut && this.autoPutSize > 0) this._AutoPut();
                else return null;
            }

            GameObject go = this._pool.Dequeue();
            go.transform.SetParent(null);
            go.SetActive(true);

            return go;
        }

        /// <summary>
        /// Get GameObject from pool
        /// </summary>
        /// <returns></returns>
        public GameObject Get(Transform parent)
        {
            if (this._pool.Count == 0)
            {
                if (this.autoPut && this.autoPutSize > 0) this._AutoPut();
                else return null;
            }

            GameObject go = this._pool.Dequeue();
            go.transform.SetParent(parent);
            go.SetActive(true);

            return go;
        }

        /// <summary>
        /// Get GameObject from pool
        /// </summary>
        /// <returns></returns>
        public GameObject Get(Transform parent, Vector3 position)
        {
            if (this._pool.Count == 0)
            {
                if (this.autoPut && this.autoPutSize > 0) this._AutoPut();
                else return null;
            }

            GameObject go = this._pool.Dequeue();
            go.transform.SetParent(parent);
            go.transform.localPosition = position;
            go.SetActive(true);

            return go;
        }

        /// <summary>
        /// Get GameObject from pool
        /// </summary>
        /// <returns></returns>
        public GameObject Get(Transform parent, Vector3 position, Quaternion rotation)
        {
            if (this._pool.Count == 0)
            {
                if (this.autoPut && this.autoPutSize > 0) this._AutoPut();
                else return null;
            }

            GameObject go = this._pool.Dequeue();
            go.transform.SetParent(parent);
            go.transform.localPosition = position;
            go.transform.rotation = rotation;
            go.SetActive(true);

            return go;
        }
    }
}
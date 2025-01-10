using Cysharp.Threading.Tasks;
using MyBox;
using OxGKit.LoggingSystem;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace OxGKit.Utilities.Pool
{
    [AddComponentMenu("OxGKit/Utilities/Pool/" + nameof(NodePool))]
    public class NodePool : MonoBehaviour
    {
        [Separator("Source options")]

        [Tooltip("The object to pool"), OverrideLabel("Source GameObject")]
        public GameObject go = null;

        [Separator("Pool options")]
        [Tooltip("Indicates whether to initialize on start")]
        public bool initializeOnStart = true;
        [Tooltip("The initial size of the pool")]
        public int initSize = 5;
        [Tooltip("Enable load smoothing across frames during initialization")]
        public bool initLoadAcrossFrames = true;
        [ConditionalField(nameof(initLoadAcrossFrames)), Tooltip("Number of frames to delay when loading during initialization")]
        public int initDelayFrame = 1;

        [Separator("Auto put options")]

        [Tooltip("If checked, the pool will automatically create objects when there are not enough available")]
        public bool autoPut = false;
        [ConditionalField(nameof(autoPut)), Tooltip("The increment count for each auto-create operation")]
        public int autoPutSize = 1;
        [ConditionalField(nameof(autoPut)), Tooltip("Enable load smoothing across frames when auto-put is enabled")]
        public bool autoPutLoadAcrossFrames = true;
        [ConditionalField(new[] { nameof(autoPut), nameof(autoPutLoadAcrossFrames) }), Tooltip("Number of frames to delay when loading with auto-put")]
        public int autoPutDelayFrame = 1;

        [Separator("Limit options")]
        [Tooltip("0 or -1 indicates no limit, while > 0 specifies the maximum allowed size of the object pool, including both initSize and autoPut, which will be restricted")]
        public int maxSize = 0;

        /// <summary>
        /// Pool objects
        /// </summary>
        private Queue<GameObject> _pool = new Queue<GameObject>();

        /// <summary>
        /// Tracks the total number of objects in the pool
        /// </summary>
        private int _currentCount = 0;

        /// <summary>
        /// Flag indicating whether the Init or Auto-put loading process has finished
        /// </summary>
        private bool _isLoadFinished = false;

        /// <summary>
        /// cancellation token
        /// </summary>
        private CancellationTokenSource _cts = null;

        private void Start()
        {
            if (this.initializeOnStart)
                this.Initialize();
        }

        private void OnDestroy()
        {
            this.Clear();
        }

        /// <summary>
        /// Init pool
        /// </summary>
        public void Initialize()
        {
            this.Clear();
            this._Init();
        }

        private void _Init()
        {
            int initialCount = this.maxSize > 0 ? Mathf.Min(this.initSize, this.maxSize) : this.initSize;
            if (this._cts == null)
                this._cts = new CancellationTokenSource();
            this._Create(initialCount, this.initLoadAcrossFrames, this.initDelayFrame, this._cts).Forget();
        }

        private void _AutoPut()
        {
            if (this.maxSize > 0 && this._currentCount >= this.maxSize)
                return;

            int availableSpace = this.maxSize > 0 ? this.maxSize - this._currentCount : this.autoPutSize;
            int createCount = Mathf.Min(this.autoPutSize, availableSpace);
            if (this._cts == null)
                this._cts = new CancellationTokenSource();
            this._Create(createCount, this.autoPutLoadAcrossFrames, this.autoPutDelayFrame, this._cts).Forget();
        }

        private async UniTaskVoid _Create(int count, bool acrossFrames, int delayFrame, CancellationTokenSource cts)
        {
            this._isLoadFinished = false;

            if (this.go == null)
                throw new ArgumentNullException("Source GameObject cannot be null. Please ensure the object pool source is specified.");

            for (int i = 0; i < count; i++)
            {
                GameObject instGo = Instantiate(this.go, Vector3.zero, Quaternion.identity, this.transform);
                this.Put(instGo);
                if (acrossFrames)
                {
                    // Load Balancing
                    await UniTask.DelayFrame(delayFrame, PlayerLoopTiming.Update, cts.Token);
                }
            }

            this._isLoadFinished = true;
        }

        /// <summary>
        /// Flag indicating whether the Init or Auto-put loading process has finished
        /// </summary>
        /// <returns></returns>
        public bool IsLoadFinished()
        {
            return this._isLoadFinished;
        }

        /// <summary>
        /// Current count of objects in the pool
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return this._currentCount;
        }

        /// <summary>
        /// Clear pool
        /// </summary>
        public void Clear()
        {
            // Cancel task
            if (this._cts != null)
            {
                this._cts.Cancel();
                this._cts.Dispose();
                this._cts = null;
            }

            // Destroy objs
            foreach (var go in this._pool)
                Destroy(go);

            // Clear pool
            this._pool.Clear();

            // Reset counter
            this._currentCount = 0;
        }

        /// <summary>
        /// Recycle GameObject
        /// </summary>
        /// <param name="go"></param>
        public void Put(GameObject go)
        {
            if (go)
            {
                if (this.maxSize > 0 && this._currentCount >= this.maxSize)
                {
                    // Destroy if exceeding maxSize
                    Destroy(go);
                    Logging.Print<Logger>("<color=#ff9d1d>Exceeded maxSize, destroying excess object for recycling.</color>");
                    return;
                }

                go.transform.SetParent(this.transform);
                go.SetActive(false);
                this._pool.Enqueue(go);
                // Increment counter
                this._currentCount++;
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
                if (this.autoPut && this.autoPutSize > 0)
                    this._AutoPut();
                else
                    return null;
            }

            GameObject go = this._pool.Dequeue();
            go.transform.SetParent(null);
            go.SetActive(true);
            // Decrement counter
            this._currentCount--;
            return go;
        }

        /// <summary>
        /// Get GameObject from pool with parent
        /// </summary>
        public GameObject Get(Transform parent)
        {
            GameObject go = this.Get();
            if (go != null)
            {
                go.transform.SetParent(parent);
            }
            return go;
        }

        /// <summary>
        /// Get GameObject from pool with parent and position
        /// </summary>
        public GameObject Get(Transform parent, Vector3 position)
        {
            GameObject go = this.Get(parent);
            if (go != null)
            {
                go.transform.localPosition = position;
            }
            return go;
        }

        /// <summary>
        /// Get GameObject from pool with parent, position, and rotation
        /// </summary>
        public GameObject Get(Transform parent, Vector3 position, Quaternion rotation)
        {
            GameObject go = this.Get(parent, position);
            if (go != null)
            {
                go.transform.rotation = rotation;
            }
            return go;
        }
    }
}
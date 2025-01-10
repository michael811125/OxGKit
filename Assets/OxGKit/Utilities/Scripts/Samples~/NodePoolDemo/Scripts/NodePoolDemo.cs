using OxGKit.Utilities.Pool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodePoolDemo : MonoBehaviour
{
    public Text poolCountTxt;
    public Text getCountTxt;

    /// <summary>
    /// Obj pool
    /// </summary>
    public NodePool objPool;

    /// <summary>
    /// Obj buffer list
    /// </summary>
    private Queue<GameObject> _objs = new Queue<GameObject>();

    private void Start()
    {
        // Manually initialize object pool
        if (this.objPool != null)
        {
            this.objPool.Initialize();
            this.poolCountTxt.text = this.objPool.Count().ToString();
        }
    }

    private void Update()
    {
        if (this.objPool != null && this.objPool.IsLoadFinished())
            this.poolCountTxt.text = this.objPool.Count().ToString();
    }

    public void GetFromPool()
    {
        if (this.objPool == null)
            return;
        var go = this.objPool.Get(this.transform);
        if (go != null)
        {
            this._objs.Enqueue(go);
            this.getCountTxt.text = this._objs.Count.ToString();
        }
    }

    public void PutIntoPool()
    {
        if (this.objPool == null)
            return;
        if (this._objs.Count > 0)
        {
            var go = this._objs.Dequeue();
            if (go != null)
            {
                this.objPool.Put(go);
                this.getCountTxt.text = this._objs.Count.ToString();
            }
        }
    }
}

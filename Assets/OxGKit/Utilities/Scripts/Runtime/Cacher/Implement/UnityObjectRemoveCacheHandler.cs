namespace OxGKit.Utilities.Cacher
{
    public class UnityObjectRemoveCacheHandler<TKey, TValue> : IRemoveCacheHandler<TKey, TValue>
    {
        public void RemoveCache(TKey key, TValue value)
        {
            if (value is UnityEngine.Object)
                UnityEngine.Object.Destroy(value as UnityEngine.Object);
        }
    }
}

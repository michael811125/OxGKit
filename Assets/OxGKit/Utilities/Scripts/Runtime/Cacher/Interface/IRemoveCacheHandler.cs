namespace OxGKit.Utilities.Cacher
{
    public interface IRemoveCacheHandler<TKey, TValue>
    {
        void RemoveCache(TKey key, TValue value);
    }
}
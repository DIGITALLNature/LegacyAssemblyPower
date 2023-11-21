// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public interface ICacheService
    {
        /// <summary>
        /// Set Cached Value with SlidingExpiration
        /// </summary>
        /// <param name="key">Key of the CachedItem</param>
        /// <param name="value">CachedItem</param>
        /// <param name="ttl">Invalidate in seconds</param>
        void SetSliding(string key, object value, int ttl);

        /// <summary>
        /// Set Cached Value with AbsoluteExpiration
        /// </summary>
        /// <param name="key">Key of the CachedItem</param>
        /// <param name="value">CachedItem</param>
        /// <param name="ttl">Invalidate in seconds</param>
        void SetAbsolute(string key, object value, int ttl);

        /// <summary>
        /// Try to read from Cache
        /// </summary>
        /// <param name="key">Key of the wanted CachedItem</param>
        /// <param name="value">CachedItem</param>
        /// <returns>CacheValue is filled</returns>
        bool TryGet(string key, out object value);

        /// <summary>
        /// Invalidate CachedItem if exists
        /// </summary>
        /// <param name="key">Key of the CachedItem to invalidate</param>
        void Remove(string key);

        /// <summary>
        /// Try to read from Cache
        /// </summary>
        object this[string key] { get; }

        /// <summary>
        /// Cache contains Key
        /// </summary>
        bool Contains(string key);
    }
}

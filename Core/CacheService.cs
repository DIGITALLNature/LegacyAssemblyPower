using System;
using System.Runtime.Caching;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class CacheService : ICacheService
    {
        public void SetSliding(string key, object value, int ttl)
        {
            if (ttl < 15)
            {
                ttl = 15;
            }
            MemoryCache.Default.Set(key, value, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(ttl) });
        }

        public void SetAbsolute(string key, object value, int ttl)
        {
            if (ttl < 15)
            {
                ttl = 15;
            }
            MemoryCache.Default.Set(key, value, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(ttl) });
        }

        public bool TryGet(string key, out object value)
        {
            value = MemoryCache.Default.Get(key);
            var hit = value != null;
            //TODO: find a smart solution to track the hitratio
            //TrackHitRatio(key, hit);
            return hit;
        }

        public void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        public object this[string key] => MemoryCache.Default[key];

        public bool Contains(string key)
        {
            return MemoryCache.Default.Contains(key);
        }

        private void TrackHitRatio(string key, bool hit)
        {
            //trace
        }
    }
}

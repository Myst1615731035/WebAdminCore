using Microsoft.Extensions.Caching.Memory;
using System;

namespace WebUtils
{
    public class MemoryCache
    {
        private static IMemoryCache _cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());

        public static T Get<T>(string key)
        {
            T value;
            if (key.IsEmpty())
            {
                return default(T);
            }
            _cache.TryGetValue<T>(key, out value);
            return value;
        }

        public static void Set<T>(string key, T value)
        {
            if (key.IsEmpty())
            {
                return;
            }
            T v;
            if(_cache.TryGetValue(key, out v)) { _cache.Remove(key); }

            _cache.Set(key, value);
        }
    }
}

using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace tinderBot.Helpers
{
    public interface IMemoryCacheHelper
    {
        T Get<T>(string key);
        void Set(string key, object data);
        bool Remove(string key);
    }

    public class MemoryCacheHelper  : IMemoryCacheHelper 
    {
        private readonly IMemoryCache _cacheService;

        public MemoryCacheHelper (IMemoryCache cacheService)
        {
            _cacheService = cacheService;
        }

        public JsonSerializerSettings CacheSerializerSettings => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException("Key of Cache is not empty or null");


            if (_cacheService.TryGetValue(key, out string result))
            {
                if (!string.IsNullOrEmpty(result))
                {
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            
            return default(T);
        }

        public void Set(string key, object data)
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException("Key of Cache is not empty or null");

            var newData = JsonConvert.SerializeObject(data, Formatting.Indented, CacheSerializerSettings);

            if (string.IsNullOrEmpty(newData))
                throw new JsonSerializationException("Object cannot convert json data for key:" + key);

            _cacheService.Set(key, newData, (MemoryCacheEntryOptions) null);
        }

        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException("Key of Cache is not empty or null");

            _cacheService.Remove(key);

            return true;
        }
    }
}

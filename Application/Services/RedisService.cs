using Application.Interfaces.Service;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase? _db;

        private static readonly ConcurrentDictionary<string, (string Value, DateTime? Expiry)> _fallbackStore
            = new();

        public RedisService(IConnectionMultiplexer? multiplexer)
        {
            try
            {
                _db = multiplexer?.GetDatabase();
            }
            catch
            {
                _db = null;
            }
        }

        public async Task StoreDataAsync(string key, string keyString, TimeSpan expiration)
        {
            if (_db == null)
            {
                StoreFallback(key, keyString, expiration);
                return;
            }

            try
            {
                await _db.StringSetAsync(key, keyString, expiration);
            }
            catch
            {
                StoreFallback(key, keyString, expiration);
            }
        }

        public async Task<long> StoreCountAsync(string key, long count, TimeSpan expiration)
        {
            if (_db == null)
            {
                return StoreCountFallback(key, count, expiration);
            }

            try
            {
                var result = await _db.StringIncrementAsync(key, count);
                await _db.KeyExpireAsync(key, expiration);
                return result;
            }
            catch
            {
                return StoreCountFallback(key, count, expiration);
            }
        }

        public async Task<bool> VerifyDataAsync(string key, string dataString)
        {
            if (_db == null)
            {
                return VerifyFallback(key, dataString);
            }

            try
            {
                var val = await _db.StringGetAsync(key);
                if (val.IsNull) return false;
                return val == dataString;
            }
            catch
            {
                return VerifyFallback(key, dataString);
            }
        }

        public async Task<bool> IsExistKeyAsync(string key)
        {
            if (_db == null)
            {
                return IsExistFallback(key);
            }

            try
            {
                return await _db.KeyExistsAsync(key);
            }
            catch
            {
                return IsExistFallback(key);
            }
        }

        public async Task DeleteDataAsync(string key)
        {
            if (_db == null)
            {
                _fallbackStore.TryRemove(key, out _);
                return;
            }

            try
            {
                await _db.KeyDeleteAsync(key);
            }
            catch
            {
                _fallbackStore.TryRemove(key, out _);
            }
        }

        public async Task<string> GetValueAsync(string key)
        {
            if (_db == null)
            {
                return GetValueFallback(key);
            }

            try
            {
                var val = await _db.StringGetAsync(key);
                return val.IsNull ? null : val.ToString();
            }
            catch
            {
                return GetValueFallback(key);
            }
        }

        #region Fallback helpers (in-memory)
        private static void StoreFallback(string key, string value, TimeSpan expiration)
        {
            var expiry = DateTime.UtcNow.Add(expiration);
            _fallbackStore[key] = (value, expiry);
        }

        private static long StoreCountFallback(string key, long count, TimeSpan expiration)
        {
            _fallbackStore.AddOrUpdate(
                key,
                k => (count.ToString(), DateTime.UtcNow.Add(expiration)),
                (k, old) =>
                {
                    if (!long.TryParse(old.Value, out var existing)) existing = 0;
                    var updated = existing + count;
                    return (updated.ToString(), DateTime.UtcNow.Add(expiration));
                });

            if (_fallbackStore.TryGetValue(key, out var entry) && long.TryParse(entry.Value, out var v))
                return v;
            return 0;
        }

        private static bool VerifyFallback(string key, string expected)
        {
            if (_fallbackStore.TryGetValue(key, out var entry) && !IsExpired(entry))
            {
                return entry.Value == expected;
            }
            _fallbackStore.TryRemove(key, out _);
            return false;
        }

        private static bool IsExistFallback(string key)
        {
            if (_fallbackStore.TryGetValue(key, out var entry) && !IsExpired(entry)) return true;
            _fallbackStore.TryRemove(key, out _);
            return false;
        }

        private static string GetValueFallback(string key)
        {
            if (_fallbackStore.TryGetValue(key, out var entry) && !IsExpired(entry)) return entry.Value;
            _fallbackStore.TryRemove(key, out _);
            return null;
        }

        private static bool IsExpired((string Value, DateTime? Expiry) entry)
        {
            return entry.Expiry.HasValue && DateTime.UtcNow > entry.Expiry.Value;
        }
        #endregion
    }
}

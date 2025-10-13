using Microsoft.Extensions.Caching.Memory;
using SignService.Common.HashSignature.Interface;
using System.Collections.Concurrent;

namespace DigitalSignService.Business.Services
{
    public class CachingService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, bool> _keys = new();

        public CachingService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SetCacheSigner(string transactionId, IHashSigner signer, string credentialId, long fileSize)
        {
            string key = $"Signer-{transactionId}";
            _keys.TryAdd(key, true);
            _cache.Set(key, (signer, credentialId, fileSize), TimeSpan.FromMinutes(20)); // change to 2 minutes
        }

        public (IHashSigner, string, long)? GetCacheSigner(string transactionId)
        {
            if (_cache.TryGetValue($"Signer-{transactionId}", out (IHashSigner, string, long)? value))
            {
                return value;
            }
            return null;
        }

        public void Remove(object key)
        {
            _cache.Remove(key);
            _keys.TryRemove(key.ToString()!, out _);
        }

        private IEnumerable<string> GetKeys(string? prefix = null)
        {
            if (string.IsNullOrEmpty(prefix))
                return _keys.Keys;

            return _keys.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        public long GetAllSignerCacheSize()
        {
            long totalSize = 0;
            foreach (var key in GetKeys("Signer-"))
            {
                if (_cache.TryGetValue(key, out (IHashSigner, string, long)? value) && value.HasValue)
                {
                    totalSize += value.Value.Item3;
                }
            }
            return totalSize;
        }
    }
}

using Microsoft.Extensions.Caching.Distributed;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions.RedisCache
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<dynamic>(this IDistributedCache cache, string recordId, dynamic data)
        {

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsonData);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);

            if (jsonData is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }        
        
        public static async Task DeleteKeyAsync(this IDistributedCache cache, string recordId)
        {
            await cache.RemoveAsync(recordId);
        }
    }
}

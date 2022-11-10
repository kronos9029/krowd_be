using Microsoft.Extensions.Caching.Distributed;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions.RedisCache
{
    public static class DistributedCacheExtensions
    {

        public static async Task<Notification> UpdateNotification(this IDistributedCache cache, string userId, NotificationDetailDTO newNoti)
        {
            NotificationDetail notification = new();
            notification.Title = newNoti.Title;
            notification.Description = newNoti.Description;
            notification.Image = newNoti.Image;
            notification.CreateDate ??= DateTimePicker.GetDateTimeByTimeZone().ToString();
            notification.Seen = false;


            Notification result = await GetRecordAsync<Notification>(cache, userId);
            if (result == null)
            {
                result = new Notification
                {
                    Details = new List<NotificationDetail>()
                };
                result.Details.Insert(0, notification);
            }
            else
            {
                result.Details.Insert(0, notification);
            }

            result.Total = result.Details.Count;
            int count = 0;
            for (int i = 0; i < result.Total; i++)
            {
                if (result.Details[i].Seen == false)
                    result.New = ++count;
                else
                    break;
            }
            await SetRecordAsync(cache, userId, result);

            return result;
        }

        public static async Task<Notification> GetNotification(this IDistributedCache cache, string userId, bool seen)
        {
            Notification result = await GetRecordAsync<Notification>(cache, userId);
            if (seen == true)
            {
                result.New = 0;
                for (int i = 0; i < result.Total; i++)
                {
                    if (result.Details[i].Seen == false)
                        result.Details[i].Seen = true;
                    else
                        break;
                }
                await SetRecordAsync(cache, userId, result);
            }

            return result;
        }

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
    }
}

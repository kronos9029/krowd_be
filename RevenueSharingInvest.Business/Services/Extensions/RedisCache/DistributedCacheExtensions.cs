﻿using Microsoft.Extensions.Caching.Distributed;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions.RedisCache
{
    public static class DistributedCacheExtensions
    {

        public static async Task<Notification> UpdateNotification(this IDistributedCache cache, string toUserId, NotificationDetailDTO newNoti)
        {
            NotificationDetail notification = new()
            {
                Title = newNoti.Title ??= "",
                EntityId = newNoti.EntityId ??= "",
                Image = newNoti.Image ??="",
                CreateDate = DateTimePicker.GetDateTimeByTimeZone().ToString(),
                Seen = false
            };
            Notification result = await GetRecordAsync<Notification>(cache, toUserId);
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

            await SetRecordAsync(cache, toUserId, result);

            return result;
        }

        public static async Task<Notification> GetNotification(this IDistributedCache cache, string userId, bool seen)
        {
            Notification result = await GetRecordAsync<Notification>(cache, userId);

            result ??= new()
                {
                    Total = 0,
                    New = 0,
                    Details = new List<NotificationDetail>()
                };

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
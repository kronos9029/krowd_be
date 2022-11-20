using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions.RedisCache
{
    public static class DeviceTokenCache
    {
        public static async Task UpdateDeviceToken(this IDistributedCache cache, string userId, string token)
        {
            try
            {
                string key = "Device-" + userId;
                DeviceToken result = await DistributedCacheExtensions.GetRecordAsync<DeviceToken>(cache, key);
                if (result == null)
                {
                    result = new DeviceToken
                    {
                        Tokens = new()
                        {
                            token
                        }
                    };
                }
                else
                {
                    result.Tokens.Add(token);
                }
                await DistributedCacheExtensions.SetRecordAsync(cache, key, result);
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public static async Task<DeviceToken> GetAvailableDevice(this IDistributedCache cache, string userId)
        {
            try
            {
                string key = "Device-" + userId;
                DeviceToken result = await DistributedCacheExtensions.GetRecordAsync<DeviceToken>(cache, key);
                var message = new MulticastMessage()
                {
                    Tokens = result.Tokens,
                };

                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message, true);

                var failedTokens = new List<string>();
                if (response.FailureCount > 0)
                {
                    for (var i = 0; i < response.Responses.Count; i++)
                    {
                        if (!response.Responses[i].IsSuccess)
                        {
                            // The order of responses corresponds to the order of the registration tokens.
                            failedTokens.Add(result.Tokens[i]);
                        }
                    }
                }
                result.Tokens = result.Tokens.Except(failedTokens).ToList();

                await DistributedCacheExtensions.SetRecordAsync(cache, key, result);
                return result;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public static async Task<List<string>> ValidateDeviceToken(List<string> newTokens)
        {
            try
            {
                var message = new MulticastMessage()
                {
                    Tokens = newTokens
                };

                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message, true);

                var failedTokens = new List<string>();
                if (response.FailureCount > 0)
                {
                    for (var i = 0; i < response.Responses.Count; i++)
                    {
                        if (!response.Responses[i].IsSuccess)
                            failedTokens.Add(newTokens[i]);
                        
                    }
                }

                return newTokens.Except(failedTokens).ToList();
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }



    }
}

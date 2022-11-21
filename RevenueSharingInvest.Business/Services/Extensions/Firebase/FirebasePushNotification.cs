using FirebaseAdmin.Messaging;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Caching.Distributed;
using RevenueSharingInvest.Business.Services.Extensions.RedisCache;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using DistributedCacheExtensions = RevenueSharingInvest.Business.Services.Extensions.RedisCache.DistributedCacheExtensions;

namespace RevenueSharingInvest.Business.Services.Extensions.Firebase
{
    public static class FirebasePushNotification
    {
        public static async Task<List<string>> SendMultiDevicePushNotification(List<string> deviceTokens, PushNotification notification)
        {
            // Create a list containing up to 500 registration tokens.
            // These registration tokens come from the client FCM SDKs.
            var message = new MulticastMessage()
            {
                Tokens = deviceTokens,
                
                Notification = new()
                {
                    Title = notification.Title,
                    Body = notification.Body,
                    ImageUrl = notification.ImageUrl
                },
            };

            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            var failedTokens = new List<string>();
            if (response.FailureCount > 0)
            {
                for (var i = 0; i < response.Responses.Count; i++)
                {
                    if (!response.Responses[i].IsSuccess)
                    {
                        // The order of responses corresponds to the order of the registration tokens.
                        failedTokens.Add(deviceTokens[i]);
                    }
                }
            }

            return failedTokens;
        }        
        
        public static async Task SendPushNotification(string deviceToken, PushNotification notification)
        {
            if (notification.ImageUrl.Equals(""))
            {
                var message = new Message()
                {
                    Token = deviceToken,
                    Notification = new()
                    {
                        Title = notification.Title,
                        Body = notification.Body
                    },
                    Android = new AndroidConfig()
                    {
                        Notification = new AndroidNotification()
                        {
                            Icon = "stock_ticker_update",
                            Color = "#f45342",
                        },
                    },
                    Apns = new ApnsConfig()
                    {
                        Aps = new Aps()
                        {
                            Badge = 42,
                        },
                    },
                };
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            } else
            {
                var message = new Message()
                {
                    Token = deviceToken,
                    Notification = new()
                    {
                        Title = notification.Title,
                        Body = notification.Body,
                        ImageUrl = notification.ImageUrl
                    },
                    Android = new AndroidConfig()
                    {
                        Notification = new AndroidNotification()
                        {
                            Icon = "stock_ticker_update",
                            Color = "#f45342",
                        },
                    },
                    Apns = new ApnsConfig()
                    {
                        Aps = new Aps()
                        {
                            Badge = 42,
                        },
                    },
                };

                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
        }
        
        public static async Task<string> SendPushNotificationToUpdateProjectTopics(string projectId, PushNotification notification)
        {
            string topic = "UpdateProject-" + projectId;
            var message = new Message()
            {
                Notification = new()
                {
                    Title = notification.Title,
                    Body = notification.Body,
                    ImageUrl = notification.ImageUrl
                },
                Topic = topic
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);;
            return topic;
        }        


        public static async Task<string> SubcribeTokensToUpdateProjectTopics(this IDistributedCache cache,DeviceToken tokens,string projectId,string userId)
        {
            var registrationTokens = tokens.Tokens;
            string topic = "UpdateProject-" + projectId;
            if(registrationTokens.Count > 0)
                await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(registrationTokens, topic);

            await DeviceTokenCache.SubcribeUserToTopic(cache, topic, userId);

            return topic;
        }

        public static async Task<string> UnsubcribeTokensToUpdateProjectTopics(this IDistributedCache cache , DeviceToken tokens, string projectId, string userId)
        {
            var registrationTokens = tokens.Tokens;
            string topic = "UpdateProject-" + projectId;
            
            await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(registrationTokens, topic);
            await DeviceTokenCache.UnsubcribeUserToTopic(cache, topic, userId);

            return topic;
        }
    }
}

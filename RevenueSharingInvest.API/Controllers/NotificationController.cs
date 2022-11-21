using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Extensions.RedisCache;
using RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs;
using System.Threading.Tasks;
using DistributedCacheExtensions = RevenueSharingInvest.Business.Services.Extensions.RedisCache.DistributedCacheExtensions;
using Notification = RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs.Notification;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/notifications")]
    [EnableCors]
    public class NotificationController : ControllerBase
    {

        private readonly IDistributedCache _distributedCache;

        public NotificationController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNoti(string userId, NotificationDetailDTO newNoti)
        {
            var result = await NotificationCache.UpdateNotification(_distributedCache, userId, newNoti);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetNoti(string userId, bool seen)
        {
            var result = await NotificationCache.GetNotification(_distributedCache, userId, seen);
            return Ok(result);
        }
    }
}

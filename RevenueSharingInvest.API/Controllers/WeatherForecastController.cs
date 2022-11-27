using Firebase.Auth;
using Firebase.Storage;
using FirebaseAdmin.Messaging;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Business.Services.Extensions.iText;
using RevenueSharingInvest.Business.Services.Extensions.RedisCache;
using RevenueSharingInvest.Business.Services.Extensions.Security;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using DistributedCacheExtensions = RevenueSharingInvest.Business.Services.Extensions.RedisCache.DistributedCacheExtensions;
using Notification = RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs.Notification;
using Project = RevenueSharingInvest.Data.Models.Entities.Project;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IITextService _iTextService;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IDistributedCache _distributedCache;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IProjectRepository _projectRepository;
        private readonly IValidationService _validationService;
        private readonly IDistributedCache _cache;
        private readonly IProjectEntityService _projectEntityService;


        public WeatherForecastController(IITextService iTextService, 
            IRoleService roleService, 
            IUserService userService,
            IProjectService projectService,
            IDistributedCache distributedCache, 
            IBackgroundJobClient backgroundJobClient, 
            IProjectRepository projectRepository,
            IValidationService validationService,
            IDistributedCache cache,
            IProjectEntityService projectEntityService)
        {

            _iTextService = iTextService;
            _roleService = roleService;
            _userService = userService;
            _projectService = projectService;
            _distributedCache = distributedCache;
            _backgroundJobClient = backgroundJobClient;
            _projectRepository = projectRepository;
            _validationService = validationService;
            _cache = cache;
            _projectEntityService = projectEntityService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNoti(string projectId, double amount, string invesmentId)
        {
            decimal result = Convert.ToDecimal(amount);
            return Ok(result);
        }

        [HttpPost]
        [Route("/test")]
        public async Task<IActionResult> rt(string secrectKey)
        {
            
            return Ok(GenerateSecurityKey.CreateKey(secrectKey));
        }

        [HttpGet]
        public async Task<IActionResult> GetNoti(string userId, bool seen)
        {
            var result = await NotificationCache.GetNotification(_distributedCache, userId, seen);
            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateProjectSchedule(Guid projectId)
        {
            Project project = await _projectRepository.GetProjectById(projectId);
            DateTime endDate = DateTime.ParseExact("19/11/2022 23:59:59", "dd/MM/yyyy HH:mm:ss", null);
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            //_backgroundJobClient.Schedule<ProjectService>(
            //                    projectService => projectService
            //                    .UpdateProjectStatusByHangfire(projectId, currentUser), TimeSpan.FromTicks(endDate.Ticks - DateTimePicker.GetDateTimeByTimeZone().Ticks));

            _backgroundJobClient.Schedule<ProjectService>(
                                projectService => projectService
                                .CreateRepaymentStageCheck(projectId, currentUser), TimeSpan.FromTicks(endDate.Ticks - DateTimePicker.GetDateTimeByTimeZone().Ticks));
            //await _projectService.CreateRepaymentStageCheck(projectId, currentUser);

            return Ok();
        }



/*        [HttpPost]
        public async Task<IActionResult> Getokok()
        {
            var registrationToken = "dNixyIUcTbK6xix5M898n1:APA91bESs3aJM2xR0I-uTWUnAVvhadd3oRVxqqI7OnssXhD7GkR6bEOPJtI-WfOgxeE1tSiyp_PSeAkUHgnfd86rNKtQgSTe4D06LPfaW5fMdE158APDccruowYZJXYYducQCBf4GuQR";
            // Create a list containing up to 500 messages.
            var messages = new List<Message>()
            {
                new Message()
                {
                    Notification = new Notification()
                    {
                        Title = "Price drop",
                        Body = "5% off all electronics",
                    },
                    Token = registrationToken,
                },
                new Message()
                {
                    Notification = new Notification()
                    {
                        Title = "Price drop",
                        Body = "2% off all books",
                    },
                    Topic = "readers-club",
                },
            };

                    var response = await FirebaseMessaging.DefaultInstance.SendAllAsync(messages);
                    // See the BatchResponse reference documentation
                    // for the contents of response.
                    return Ok(response);
                }*/

        /*        private string CreateSignature(string message, string key)
                {
                    try
                    {
                        byte[] keyByte = Encoding.UTF8.GetBytes(key);
                        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                        using var hmacsha256 = new HMACSHA256(keyByte);
                        byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                        string hex = BitConverter.ToString(hashmessage);
                        hex = hex.Replace("-", "").ToLower();
                        return hex;
                    }
                    catch (Exception e)
                    {
                        LoggerService.Logger(e.ToString());
                        throw new Exception(e.Message);
                    }

                }

                private string GenerateAccessKey()
                {
                    var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var Charsarr = new char[16];
                    var random = new Random();

                    for (int i = 0; i < Charsarr.Length; i++)
                    {
                        Charsarr[i] = characters[random.Next(characters.Length)];
                    }

                    var resultString = new String(Charsarr);
                    return resultString;
                }

                private string GenerateSecretKey()
                {
                    var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var Charsarr = new char[32];
                    var random = new Random();

                    for (int i = 0; i < Charsarr.Length; i++)
                    {
                        Charsarr[i] = characters[random.Next(characters.Length)];
                    }

                    var resultString = new String(Charsarr);
                    return resultString;
                }*/

    }

}

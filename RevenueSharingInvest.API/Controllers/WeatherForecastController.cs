using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly FirestoreProvider _firestoreProvider;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IProjectService _projectService;
        private readonly AppSettings _appSettings;

        public WeatherForecastController(FirestoreProvider firestoreProvider,
            IUserService userService,
            IRoleService roleService,
            IOptions<AppSettings> appSettings,
            IProjectService projectService)
        {
            _firestoreProvider = firestoreProvider;
            _userService = userService;
            _roleService = roleService;
            _appSettings = appSettings.Value;
            _projectService = projectService;
        }

        [HttpPost]
        public async Task<IActionResult> Getokok(ClientUploadRevenueRequest request)
        {
            string accessKey = GenerateAccessKey();
            string secretKey = GenerateSecretKey();


            //IntegrateInfo info = await _projectService.GetIntegrateInfoByUserEmail(request.projectId);

            //string userMessage = "projectId=" + request.projectId + "&accessKey=" + request.accessKey;
            //string systemMessage = "projectId=" + info.ProjectId + "&accessKey=" + info.AccessKey;

            //string userSignature = CreateSignature(userMessage, info.SecretKey);
            //string systemSignature = CreateSignature(systemMessage, info.SecretKey);

            //bool check = userSignature.Equals(systemMessage);

            return Ok(0);
        }

        private string CreateSignature(string message, string key)
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
        }

    }
}

﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/public/v1.0/POS")]
    [EnableCors]
    public class POSController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IProjectService _projectService;
        private readonly AppSettings _appSettings;
        private readonly IBillService _billService;

        public POSController(IUserService userService,
            IRoleService roleService,
            IOptions<AppSettings> appSettings,
            IProjectService projectService,
            IBillService billService)
        {
            _userService = userService;
            _roleService = roleService;
            _appSettings = appSettings.Value;
            _projectService = projectService;
            _billService = billService;
        }

        [HttpPost]
        [Route("Krowd-upload")]
        public async Task<IActionResult> UploadBillsFromKrowd(KrowdUploadRevenueRequest request)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            InsertBillDTO billDTO = new();

            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                billDTO.projectId = request.projectId;
                billDTO.bills = request.bills;
                var result = await _billService.BulkInsertBills(billDTO, billDTO.projectId, request.date);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
        }

        [HttpPost]
        [Route("Client-upload")]
        public async Task<IActionResult> UploadBillsFromPOS(ClientUploadRevenueRequest request)
        {
            IntegrateInfo info = await _projectService.GetIntegrateInfoByUserEmail(request.projectId);

            string systemMessage = "projectId="+info.ProjectId+"&accessKey="+info.AccessKey;
            
            string systemSignature = CreateSignature(systemMessage, info.SecretKey);

            if (systemSignature.Equals(request.signature))
            {
                InsertBillDTO billDTO = new();
                billDTO.projectId = info.ProjectId.ToString();
                billDTO.bills = request.bills;
                var result = await _billService.BulkInsertBills(billDTO, billDTO.projectId, request.date);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, "You Don't Have Permission Perform This Action!!");
        }
/*
        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetBillsDate(string projectId)
        {
            var result = await _firestoreProvider.GetDatesOfProject(projectId);
            return Ok(result);
        }
        //GET ALL
        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> GetTest(string projectId, string date)
        {
            var result = await _firestoreProvider.GetInvoiceDetailByDate(projectId, date);
            return Ok(result);
        }*/

        [HttpPost]
        [Route("generate-sig")]
        public string CreateSignature(string message, string key)
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
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }

    }

    public class ClientUploadRevenueRequest
    {
        public string projectId { get; set; }
        public string date { get; set; }
        public string signature { get; set; }
        public List<BillDTO> bills { get; set; }
    }

    public class KrowdUploadRevenueRequest
    {
        public string projectId { get; set; }
        public string date { get; set; }
        public List<BillDTO> bills { get; set; }
    }
}

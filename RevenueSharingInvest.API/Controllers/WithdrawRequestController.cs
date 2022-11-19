using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/withdraw_requests")]
    [EnableCors]
    [Authorize]
    public class WithdrawRequestController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IWithdrawRequestService _withdrawRequestService;
        private readonly IFileUploadService _uploadService;
        private readonly IValidationService _validationService;
        public WithdrawRequestController(IUserService userService,
            IRoleService roleService,
            IWithdrawRequestService withdrawRequestService,
            IFileUploadService fileUploadService,
            IValidationService validationService)
        {
            _userService = userService;
            _roleService = roleService;
            _withdrawRequestService = withdrawRequestService;
            _uploadService = fileUploadService;
            _validationService = validationService;
        }

        //GET ALL
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetWithdrawRequestByUserId(int pageIndex, int pageSize, string userId, WithdrawRequestEnum filter)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                if (userId != null && !userId.Equals(Guid.Parse(currentUser.userId))) throw new InvalidFieldException("This userId is not your userId!!!");

                var result = await _withdrawRequestService.GetAllWithdrawRequest(pageIndex, pageSize, userId == null ? currentUser.userId : userId.ToString(), filter.ToString());
                return Ok(result);
            } else if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                if (userId != null)
                {
                    GetUserDTO user = await _userService.GetUserById(Guid.Parse(userId));
                    if (!Guid.Parse(user.role.id).Equals(Guid.Parse(RoleDictionary.role.GetValueOrDefault("INVESTOR")))) throw new InvalidFieldException("userId must belong to an INVESTOR!!!");
                }                    

                var result = await _withdrawRequestService.GetAllWithdrawRequest(pageIndex, pageSize, userId, filter.ToString());
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or INVESTOR can perform this action!!!");
        }

        //GET BY ID
        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> GetWithdrawRequestById(string id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _withdrawRequestService.GetWithdrawRequestById(id);
                return Ok(result);
            }
            else
            {
                var result = await _withdrawRequestService.GetWithdrawRequestByRequestIdAndUserId(id, currentUser.userId);
                return Ok(result);
            }
            
            
        }

        //CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateWithdrawRequest([FromBody] WithdrawRequestDTO request)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if(currentUser.roleId.Equals(currentUser.investorRoleId) || currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _withdrawRequestService.CreateWithdrawRequest(request, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role Investor or Project Manager can perform this action!!!");
        }


        //UPDATE
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateWithdrawRequest([FromForm] UpdateWithdrawRequest request, WithdrawAction action)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            var currentRequest = await _withdrawRequestService.GetWithdrawRequestByRequestIdAndUserId(request.requestId
                , currentUser.roleId.Equals(currentUser.adminRoleId) ? (await _withdrawRequestService.GetWithdrawRequestById(request.requestId)).CreateBy : currentUser.userId);

            if (currentRequest == null)
                throw new NotFoundException("No Such Request With This Request ID!!");

            if(currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                if (WithdrawAction.APPROVE.ToString().Equals(action.ToString()))
                {
                    if (currentRequest.Status.Equals(WithdrawRequestEnum.PENDING.ToString()))
                    {
                        string receiptLink = await _uploadService.UploadAdminTracsactionReceipt(request.requestId, request.receipt, currentUser.userId);
                        var result = await _withdrawRequestService.AdminApproveWithdrawRequest(currentUser, currentRequest, receiptLink);
                        return Ok(result);
                    } 
                    else if (currentRequest.Status.Equals(WithdrawRequestEnum.PARTIAL_ADMIN.ToString()))
                    {
                        string receiptLink = await _uploadService.UploadAdminTracsactionReceipt(request.requestId, request.receipt, currentUser.userId);
                        var result = await _withdrawRequestService.AdminResponeToWithdrawRequest(currentUser, currentRequest, receiptLink);
                        return Ok(result);
                    }
                } 
                else if (WithdrawAction.REJECT.ToString().Equals(action.ToString()))
                {
                    if (currentRequest.Status.Equals(WithdrawRequestEnum.PENDING.ToString()))
                    {
                        var result = await _withdrawRequestService.AdminRejectWithdrawRequest(currentUser, currentRequest, request.refusalReason);
                        return Ok(result);
                    }

                }
            } 
            else if (currentUser.roleId.Equals(currentUser.investorRoleId) || currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                if (WithdrawAction.REPORT.ToString().Equals(action.ToString()))
                {
                    if (currentRequest.Status.Equals(WithdrawRequestEnum.PARTIAL.ToString()))
                    {
                        var result = await _withdrawRequestService.ReportWithdrawRequest(currentUser, currentRequest, request.reportMessage);
                        return Ok(result);
                    }
                }
                else if (WithdrawAction.APPROVE.ToString().Equals(action.ToString()))
                {
                    if (currentRequest.Status.Equals(WithdrawRequestEnum.PARTIAL.ToString()))
                    {
                        var result = await _withdrawRequestService.ApproveWithdrawRequest(currentUser.userId, currentRequest);
                        return Ok(result);
                    }
                }
            }
            
            return StatusCode((int)HttpStatusCode.Forbidden, "ERROR");
        }
    }

    public enum WithdrawAction
    {
        APPROVE,
        REJECT,
        REPORT
    }
}

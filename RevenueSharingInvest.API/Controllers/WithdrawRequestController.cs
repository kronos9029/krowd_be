using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/WithdrawRequest")]
    [EnableCors]
    [Authorize]
    public class WithdrawRequestController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IWithdrawRequestService _withdrawRequestService;
        public WithdrawRequestController(IUserService userService,
            IRoleService roleService,
            IWithdrawRequestService withdrawRequestService)
        {
            _userService = userService;
            _roleService = roleService;
            _withdrawRequestService = withdrawRequestService;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetWithdrawRequestByUserId(string investorId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _withdrawRequestService.GetWithdrawRequestByUserId(currentUser.userId);
                return Ok(result);
            } else if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                if (!investorId.Equals("") || investorId != null)
                {
                    var result = await _withdrawRequestService.GetWithdrawRequestByUserId(investorId);
                    return Ok(result);
                } else
                {
                    var result = await _withdrawRequestService.AdminGetAllWithdrawRequest();
                    return Ok(result);
                }
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "");
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> GetWithdrawRequestById(string requestId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            var result = await _withdrawRequestService.GetWithdrawRequestByRequestIdAndUserId(requestId, currentUser.userId);
            return Ok(result);
        }

        //CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateWithdrawRequest([FromBody] InvestorWithdrawRequest request)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if(currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _withdrawRequestService.CreateInvestorWithdrawRequest(request, currentUser);
                return Ok(result);
            }
            
            return StatusCode((int)HttpStatusCode.Forbidden, "");
        }


        //PUT
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateWithdrawRequest([FromBody] UpdateWithdrawRequest request, WithdrawAction action)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            var currentRequest = await _withdrawRequestService.GetWithdrawRequestByRequestIdAndUserId(request.requestId, currentUser.userId);
            if (currentRequest == null)
                throw new NotFoundException("No Such Request With This Request ID!!");

            if(currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                if (WithdrawAction.APPROVE.ToString().Equals(action))
                {
                    if (currentRequest.Status.Equals(WithdrawRequestEnum.PENDING.ToString()))
                    {
                        var result = await _withdrawRequestService.AdminApproveWithdrawRequest(currentUser, currentRequest.Id, currentRequest.Amount);
                        return Ok(result);
                    } 
                    else if (currentRequest.Status.Equals(WithdrawRequestEnum.PARTIAL_ADMIN.ToString()))
                    {
                        var result = await _withdrawRequestService.AdminResponeToWithdrawRequest(currentUser, currentRequest.Id);
                        return Ok(result);
                    }
                } 
                else if (WithdrawAction.REJECT.ToString().Equals(action))
                {
                    if (currentRequest.Status.Equals(WithdrawRequestEnum.PENDING.ToString()))
                    {
                        var result = await _withdrawRequestService.AdminRejectWithdrawRequest(currentUser.userId, currentRequest.Id, request.refusalReason);
                        return Ok(result);
                    }

                }
            } 
            else if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                if (WithdrawAction.REPORT.ToString().Equals(action))
                {
                    if (currentRequest.Status.Equals(WithdrawRequestEnum.PARTIAL.ToString()))
                    {
                        var result = await _withdrawRequestService.AdminRejectWithdrawRequest(currentUser.userId, currentRequest.Id, request.refusalReason);
                        return Ok(result);
                    }
                }
                else if (WithdrawAction.APPROVE.ToString().Equals(action))
                {
                    if (currentRequest.Status.Equals(WithdrawRequestEnum.PARTIAL.ToString()))
                    {
                        var result = await _withdrawRequestService.InvestorApproveWithdrawRequest(currentUser.userId, currentRequest.Id);
                        return result;
                    }
                }


            }
            
            return StatusCode((int)HttpStatusCode.Forbidden, "");
        }
    }

    public enum WithdrawAction
    {
        APPROVE,
        REJECT,
        REPORT
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/investments")]
    [EnableCors]
    public class InvestmentController : ControllerBase
    {
        private readonly IInvestmentService _investmentService;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public InvestmentController(IInvestmentService investmentService, IRoleService roleService, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _investmentService = investmentService;
            _roleService = roleService;
            _userService = userService;
            this.httpContextAccessor = httpContextAccessor;
        }

        //CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateInvestment([FromBody] CreateInvestmentDTO investmentDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _investmentService.CreateInvestment(investmentDTO, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role INVESTOR can perform this action!!!");
        }

        //GET ALL
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllInvestments(int pageIndex, int pageSize, string walletTypeId, string businessId, string projectId, string investorId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = new List<GetInvestmentDTO>();
                result = await _investmentService.GetAllInvestments(pageIndex, pageSize, walletTypeId, businessId, projectId, investorId, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or BUINESS_MANAGER or PROJECT_MANAGER or INVESTOR can perform this action!!!");
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> GetInvestmentById(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                InvestmentDTO dto = new GetInvestmentDTO();
                dto = await _investmentService.GetInvestmentById(id, currentUser);
                return Ok(dto);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or BUINESS_MANAGER or PROJECT_MANAGER or INVESTOR can perform this action!!!");
        }

        //[HttpGet]
        //[Route("wallet/{walletType}")]
        //[Authorize]
        //public async Task<IActionResult> GetInvestmentByForWallet(string walletType)
        //{
        //    ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

        //    if (currentUser.roleId.Equals(currentUser.investorRoleId))
        //    {
        //        var result = new List<GetInvestmentDTO>();
        //        result = await _investmentService.GetInvestmentForWallet(walletType, currentUser);
        //        return Ok(result);
        //    }
        //    return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role INVESTOR can perform this action!!!");
        //}
    }
}

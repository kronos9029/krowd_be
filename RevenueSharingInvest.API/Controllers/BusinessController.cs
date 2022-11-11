using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.Constants;
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
    [Route("api/v1.0/businesses")]
    [EnableCors]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _businessService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        public BusinessController(IBusinessService businessService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, IUserService userService)
        {
            _businessService = businessService;
            _httpContextAccessor = httpContextAccessor;
            _roleService = roleService;
            _userService = userService;
        }

        //CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBusiness([FromForm] CreateBusinessDTO businessDTO, [FromQuery] List<string> fieldIdList)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if(currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _businessService.CreateBusiness(businessDTO, fieldIdList, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN can perform this action!!!");
        }

        //GET ALL
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBusinesses(int pageIndex, int pageSize, string status, string name, string orderBy, string order)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if(currentUser.roleId.Equals(currentUser.adminRoleId) 
                || (currentUser.roleId.Equals(currentUser.investorRoleId) && !currentUser.investorId.Equals("")) 
                || currentUser.roleId.Equals(""))
            {
                var result = new AllBusinessDTO();
                result = await _businessService.GetAllBusiness(pageIndex, pageSize, status, name, orderBy, order, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or INVESTOR or GUEST can perform this action!!!");
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetBusinessById([FromRoute]string id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            GetBusinessDTO dto = new GetBusinessDTO();

            if((currentUser.roleId.Equals(currentUser.businessManagerRoleId) || currentUser.roleId.Equals(currentUser.projectManagerRoleId)) && !currentUser.businessId.Equals(""))
            {
                dto = await _businessService.GetBusinessById(Guid.Parse(currentUser.businessId));
                return Ok(dto);
            } else if (currentUser.roleId.Equals(currentUser.adminRoleId) && id != null)
            {
                dto = await _businessService.GetBusinessById(Guid.Parse(id));
                return Ok(dto);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Do Not Have Permission To Access This Business!!");
        }

        //UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateBusiness([FromForm] UpdateBusinessDTO businessDTO, Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //ADMIN, BUSINESS_MANAGER
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId))
            {
                var result = await _businessService.UpdateBusiness(businessDTO, id, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "You Do Not Have Permission To Access This Business!!");
        }

        //UPDATE STATUS
        [HttpPut]
        [Route("status/{id},{status}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateProjectStatus(Guid id, string status)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _businessService.UpdateBusinessStatus(id, status, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN can perform this action!!!");
        }

        //DELETE BY ID
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBusiness(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _businessService.DeleteBusinessById(id, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN can perform this action!!!");
        }

        //CLEAR DATA
        //[HttpDelete]
        //public async Task<IActionResult> ClearAllBusinessData()
        //{
        //    ThisUserObj userInfo = await GetThisUserInfo(HttpContext);

        //    if (userInfo.roleId.Equals(userInfo.adminRoleId))
        //    {
        //        var result = await _businessService.ClearAllBusinessData();
        //        return Ok(result);
        //    }

        //    return StatusCode((int)HttpStatusCode.Forbidden, "You Do Not Have Permission To Access This Business!!");
        //}

    }
}

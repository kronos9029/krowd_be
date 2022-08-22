using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/v1.0/investors")]
    [EnableCors]
    //[Authorize]
    public class InvestorController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IInvestorService _investorService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public InvestorController(IUserService userService, IRoleService roleService, IInvestorService investorService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _roleService = roleService;
            _investorService = investorService;
            this.httpContextAccessor = httpContextAccessor;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateInvestor([FromBody] InvestorDTO investorDTO)
        //{
        //    var result = await _investorService.CreateInvestor(investorDTO);
        //    return Ok(result);
        //}

        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> GetAllInvestors(int pageIndex, int pageSize, string status, string name, string orderBy, string order)
        //{
        //    ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

        //    if (currentUser.roleId.Equals(currentUser.adminRoleId)
        //        || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
        //        || currentUser.roleId.Equals(currentUser.projectManagerRoleId))
        //    {
        //        var result = new List<InvestorDTO>();
        //        result = await _investorService.GetAllInvestors(pageIndex, pageSize);
        //        return Ok(result);
        //    }
        //    return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or BUSINESS_MANAGER or PROJECT_MANAGER can perform this action!!!");
        //}

        //[HttpGet]
        //[Route("{id}")]
        //[Authorize]
        //public async Task<IActionResult> GetInvestorById(Guid id)
        //{
        //    ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

        //    if (currentUser.roleId.Equals(currentUser.adminRoleId)
        //        || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
        //        || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
        //        || currentUser.roleId.Equals(currentUser.investorRoleId))
        //    {

        //    }
        //    InvestorDTO dto = new InvestorDTO();
        //    dto = await _investorService.GetInvestorById(id);
        //    return Ok(dto);
        //}

        //[HttpPut]
        //[Route("{id}")]
        //public async Task<IActionResult> UpdateInvestor([FromBody] InvestorDTO investorDTO, Guid id)
        //{
        //    var result = await _investorService.UpdateInvestor(investorDTO, id);
        //    return Ok(result);
        //}

        //[HttpDelete]
        //[Route("{id}")]
        //public async Task<IActionResult> DeleteInvestor(Guid id)
        //{
        //    var result = await _investorService.DeleteInvestorById(id);
        //    return Ok(result);
        //}

        //[HttpDelete]
        //public async Task<IActionResult> ClearAllInvestorData()
        //{
        //    var result = await _investorService.ClearAllInvestorData();
        //    return Ok(result);
        //}
        private async Task<ThisUserObj> GetThisUserInfo(HttpContext? httpContext)
        {
            ThisUserObj currentUser = new();

            var checkUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber);
            if (checkUser == null)
            {
                currentUser.userId = "";
                currentUser.email = "";
                currentUser.investorId = "";
            }
            else
            {
                currentUser.userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;
                currentUser.email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                currentUser.investorId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GroupSid).Value;
            }

            List<RoleDTO> roleList = await _roleService.GetAllRoles();
            GetUserDTO? userDTO = await _userService.GetUserByEmail(currentUser.email);
            if (userDTO == null)
            {
                currentUser.roleId = "";
                currentUser.businessId = "";

            }
            else
            {
                if (userDTO.business != null)
                {
                    currentUser.roleId = userDTO.role.id;
                    currentUser.businessId = userDTO.business.id;
                }
                else
                {
                    currentUser.roleId = userDTO.role.id;
                    currentUser.businessId = "";
                }

            }


            foreach (RoleDTO role in roleList)
            {
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(0)))
                {
                    currentUser.adminRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(3)))
                {
                    currentUser.investorRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(1)))
                {
                    currentUser.businessManagerRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(2)))
                {
                    currentUser.projectManagerRoleId = role.id;
                }
            }

            return currentUser;
        }
    }
}

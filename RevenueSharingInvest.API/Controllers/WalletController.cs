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
    [Route("api/v1.0/wallets")]
    [EnableCors]
    //[Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IProjectWalletService _projectWalletService;
        private readonly IInvestorWalletService _investorWalletService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public WalletController(IInvestorWalletService investorWalletService, IProjectWalletService projectWalletService, IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _roleService = roleService;
            _investorWalletService = investorWalletService;
            _projectWalletService = projectWalletService;
            this.httpContextAccessor = httpContextAccessor;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateInvestorWallet([FromBody] InvestorWalletDTO investorWalletDTO)
        //{
        //    var result = await _investorWalletService.CreateInvestorWallet(investorWalletDTO);
        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllWallets()
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _investorWalletService.GetAllInvestorWallets(currentUser);
                return Ok(result);
            }

            else if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _projectWalletService.GetAllProjectWallets(currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role INVESTOR or PROJECT_MANAGER can perform this action!!!");
        }

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

        //[HttpGet]
        //[Route("{id}")]
        //public async Task<IActionResult> GetInvestorWalletById(Guid id)
        //{
        //    InvestorWalletDTO dto = new InvestorWalletDTO();
        //    dto = await _investorWalletService.GetInvestorWalletById(id);
        //    return Ok(dto);
        //}

        //[HttpPut]
        //[Route("{id}")]
        //public async Task<IActionResult> UpdateInvestorWallet([FromBody] InvestorWalletDTO investorWalletDTO, Guid id)
        //{
        //    var result = await _investorWalletService.UpdateInvestorWallet(investorWalletDTO, id);
        //    return Ok(result);
        //}

        //[HttpDelete]
        //[Route("{id}")]
        //public async Task<IActionResult> DeleteInvestorWallet(Guid id)
        //{
        //    var result = await _investorWalletService.DeleteInvestorWalletById(id);
        //    return Ok(result);
        //}

        //[HttpDelete]
        //public async Task<IActionResult> ClearAllInvestorWalletData()
        //{
        //    var result = await _investorWalletService.ClearAllInvestorWalletData();
        //    return Ok(result);
        //}
    }
}

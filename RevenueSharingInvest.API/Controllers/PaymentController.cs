using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.Constants.Enum;
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
    [Route("api/v1.0/payments")]
    [EnableCors]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public PaymentController(IPaymentService paymentService, IRoleService roleService, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _paymentService = paymentService;
            _roleService = roleService;
            _userService = userService;
            this.httpContextAccessor = httpContextAccessor;
        }

        //GET ALL
        [HttpGet]
        [Route("type/{type}")]
        [Authorize]
        public async Task<IActionResult> GetAllPayments(int pageIndex, int pageSize, PaymentTypeEnum type)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || (currentUser.roleId.Equals(currentUser.investorRoleId) && !currentUser.investorId.Equals("")))
            {
                var result = await _paymentService.GetAllPayments(pageIndex, pageSize, type.ToString(), currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_OWNER INVESTOR can perform this action!!!");            
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || (currentUser.roleId.Equals(currentUser.investorRoleId) && !currentUser.investorId.Equals("")))
            {
                //GetPaymentDTO dto = new GetPaymentDTO();
                //dto = await _paymentService.GetPaymentById(id, currentUser);
                //return Ok(dto);
                return Ok();
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_OWNER INVESTOR can perform this action!!!");
            
        }
    }
}

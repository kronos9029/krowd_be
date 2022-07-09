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
    [Route("api/v1.0/areas")]
    [EnableCors]
    //[Authorize]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuthenticateService _authenticateService;
        public AreaController(IAreaService areaService, IHttpContextAccessor httpContextAccessor, IAuthenticateService authenticateService)
        {
            _areaService = areaService;
            this.httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] AreaDTO areaDTO)
        {
            string userId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;

            //if (await _authenticateService.CheckRoleForAction(userId, RoleEnum.ADMIN.ToString()))
            //{
                var result = await _areaService.CreateArea(areaDTO);
                return Ok(result);
            //}
            //return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAreas(int pageIndex, int pageSize)
        {
            var result = new List<AreaDTO>();
            result = await _areaService.GetAllAreas(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAreaById(Guid id)
        {
            AreaDTO dto = new AreaDTO();
            dto = await _areaService.GetAreaById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateArea([FromBody] AreaDTO areaDTO, Guid id)
        {
            var result = await _areaService.UpdateArea(areaDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteArea(Guid id)
        {
            var result = await _areaService.DeleteAreaById(id);
            return Ok(result);
        }

        //[HttpDelete]
        //public async Task<IActionResult> ClearAllAreaData()
        //{
        //    var result = await _areaService.ClearAllAreaData();
        //    return Ok(result);
        //}
    }
}

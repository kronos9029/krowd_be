using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public AreaController(IAreaService areaService, IHttpContextAccessor httpContextAccessor)
        {
            _areaService = areaService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] AreaDTO areaDTO)
        {
            var result = await _areaService.CreateArea(areaDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAreas()
        {
            var result = new List<AreaDTO>();
            result = await _areaService.GetAllAreas();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateArea([FromBody] AreaDTO areaDTO, [FromQuery] Guid areaId)
        {
            var result = await _areaService.UpdateArea(areaDTO, areaId);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteArea([FromQuery] Guid areaId)
        {
            var result = await _areaService.DeleteAreaById(areaId);
            return Ok(result);
        }
    }
}

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
    [Route("api/v1.0/stages")]
    [EnableCors]
    //[Authorize]
    public class StageController : ControllerBase
    {
        private readonly IStageService _stageService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public StageController(IStageService stageService, IHttpContextAccessor httpContextAccessor)
        {
            _stageService = stageService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStage([FromBody] StageDTO stageDTO)
        {
            var result = await _stageService.CreateStage(stageDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStages()
        {
            var result = new List<StageDTO>();
            result = await _stageService.GetAllStages();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetStageById(Guid id)
        {
            StageDTO dto = new StageDTO();
            dto = await _stageService.GetStageById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateStage([FromBody] StageDTO stageDTO, Guid id)
        {
            var result = await _stageService.UpdateStage(stageDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteStage(Guid id)
        {
            var result = await _stageService.DeleteStageById(id);
            return Ok(result);
        }
    }
}

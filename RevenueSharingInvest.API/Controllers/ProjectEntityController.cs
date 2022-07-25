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
    [Route("api/v1.0/project_entities")]
    [EnableCors]
    //[Authorize]
    public class ProjectEntityController : ControllerBase
    {
        private readonly IProjectEntityService _projectEntityService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public ProjectEntityController(IProjectEntityService projectEntityService, IHttpContextAccessor httpContextAccessor)
        {
            _projectEntityService = projectEntityService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectEntity([FromForm] CreateUpdateProjectEntityDTO projectEntityDTO)
        {
            var result = await _projectEntityService.CreateProjectEntity(projectEntityDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjectEntities(int pageIndex, int pageSize)
        {
            var result = new List<GetProjectEntityDTO>();
            result = await _projectEntityService.GetAllProjectEntities(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProjectEntityById(Guid id)
        {
            GetProjectEntityDTO dto = new GetProjectEntityDTO();
            dto = await _projectEntityService.GetProjectEntityById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProjectEntity([FromForm] CreateUpdateProjectEntityDTO projectEntityDTO, Guid id)
        {
            var result = await _projectEntityService.UpdateProjectEntity(projectEntityDTO, id);
            return Ok(result);
        }

        //[HttpPut]
        //public async Task<IActionResult> UpdateProjectEntityPriority([FromQuery] List<string> idList)
        //{
        //    var result = await _projectEntityService.UpdateProjectEntityPriority(idList);
        //    return Ok(result);
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateProjectEntityPriority([FromBody] List<ProjectEntityUpdateDTO> idList)
        {
            var result = await _projectEntityService.UpdateProjectEntityPriority(idList);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProjectEntity(Guid id)
        {
            var result = await _projectEntityService.DeleteProjectEntityById(id);
            return Ok(result);
        }
    }
}

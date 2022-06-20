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
    [Route("api/v1.0/projects")]
    [EnableCors]
    //[Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public ProjectController(IProjectService projectService, IHttpContextAccessor httpContextAccessor)
        {
            _projectService = projectService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDTO projectDTO)
        {
            var result = await _projectService.CreateProject(projectDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects(int pageIndex, int pageSize, Guid businessId, string temp_field_role)
        {
            var result = new AllProjectDTO();
            result = await _projectService.GetAllProjects(pageIndex, pageSize, businessId, temp_field_role);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            ProjectDTO dto = new ProjectDTO();
            dto = await _projectService.GetProjectById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDTO projectDTO, Guid id)
        {
            var result = await _projectService.UpdateProject(projectDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var result = await _projectService.DeleteProjectById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllProjectData()
        {
            var result = await _projectService.ClearAllProjectData();
            return Ok(result);
        }
    }
}

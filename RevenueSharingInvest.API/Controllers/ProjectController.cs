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

        [HttpGet]
        public async Task<IActionResult> GetAllProject()
        {
            var result = new List<Project>();
            result = await _projectService.GetAllProjects();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDTO newProjectDTO)
        {
            var result = await _projectService.CreateProject(newProjectDTO);
            return Ok(result);
        }
    }
}

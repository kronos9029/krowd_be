﻿using Microsoft.AspNetCore.Authorization;
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
    [Route("api/v1.0/projectEntities")]
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
        public async Task<IActionResult> CreateProjectEntity([FromBody] ProjectEntityDTO projectEntityDTO)
        {
            var result = await _projectEntityService.CreateProjectEntity(projectEntityDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjectEntitys()
        {
            var result = new List<ProjectEntityDTO>();
            result = await _projectEntityService.GetAllProjectEntitys();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProjectEntityById(Guid id)
        {
            ProjectEntityDTO dto = new ProjectEntityDTO();
            dto = await _projectEntityService.GetProjectEntityById(id);
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProjectEntity([FromBody] ProjectEntityDTO projectEntityDTO, [FromQuery] Guid projectEntityId)
        {
            var result = await _projectEntityService.UpdateProjectEntity(projectEntityDTO, projectEntityId);
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
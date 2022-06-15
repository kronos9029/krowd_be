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
    [Route("api/v1.0/Project_Wallets")]
    [EnableCors]
    //[Authorize]
    public class ProjectWalletController : ControllerBase
    {
        private readonly IProjectWalletService _projectWalletService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public ProjectWalletController(IProjectWalletService projectWalletService, IHttpContextAccessor httpContextAccessor)
        {
            _projectWalletService = projectWalletService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectWallet([FromBody] ProjectWalletDTO projectWalletDTO)
        {
            var result = await _projectWalletService.CreateProjectWallet(projectWalletDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjectWallets(int pageIndex, int pageSize)
        {
            var result = new List<ProjectWalletDTO>();
            result = await _projectWalletService.GetAllProjectWallets(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProjectWalletById(Guid id)
        {
            ProjectWalletDTO dto = new ProjectWalletDTO();
            dto = await _projectWalletService.GetProjectWalletById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProjectWallet([FromBody] ProjectWalletDTO projectWalletDTO, Guid id)
        {
            var result = await _projectWalletService.UpdateProjectWallet(projectWalletDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProjectWallet(Guid id)
        {
            var result = await _projectWalletService.DeleteProjectWalletById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllWalletServiceData()
        {
            var result = await _projectWalletService.ClearAllProjectWalletData();
            return Ok(result);
        }
    }
}

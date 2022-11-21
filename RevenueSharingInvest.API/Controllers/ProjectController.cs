 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
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
    [Route("api/v1.0/projects")]
    [EnableCors]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticateService _authenticateService;
        private readonly IRoleService _roleService;
        private readonly IBusinessService _businessService;
        private readonly IUserService _userService;
        private readonly IInvestmentService _investmentService;
        public ProjectController(IProjectService projectService, 
            IHttpContextAccessor httpContextAccessor, 
            IAuthenticateService authenticateService,
            IRoleService roleService,
            IBusinessService businessService,
            IUserService userService,
            IInvestmentService investmentService)
        {
            _projectService = projectService;
            _httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
            _roleService = roleService;
            _businessService = businessService;
            _userService = userService;
            _investmentService = investmentService;
        }

        //CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProject([FromForm] CreateProjectDTO projectDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _projectService.CreateProject(projectDTO, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role BUSINESS_MANAGER can perform this action!!!");
        }

        //GET ALL
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProjects(
            bool countOnly,
            int pageIndex,
            int pageSize,
            string businessId,
            string areaId,
            [FromQuery] List<string> listFieldId,
            double investmentTargetCapital,
            string name,
            string status
            )
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            if (countOnly)
            {
                var countResult = new ProjectCountDTO();
                if(!currentUser.roleId.Equals(""))
                {
                    RoleDTO roleDTO = await _roleService.GetRoleById(Guid.Parse(currentUser.roleId));
                    countResult = await _projectService.CountProjects(businessId, areaId, listFieldId, investmentTargetCapital, name, status, currentUser);
                    return Ok(countResult);
                } else
                {
                    countResult = await _projectService.CountProjects(businessId, areaId, listFieldId, investmentTargetCapital, name, status, currentUser);
                    return Ok(countResult);
                }
            }
            else
            {
                var resultProjectList = new AllProjectDTO();
                if(!currentUser.roleId.Equals(""))
                {
                    RoleDTO roleDTO = await _roleService.GetRoleById(Guid.Parse(currentUser.roleId));

                    resultProjectList = await _projectService.GetAllProjects(pageIndex, pageSize, businessId, areaId, listFieldId, investmentTargetCapital, name, status, currentUser);
                    return Ok(resultProjectList);
                } else
                {
                    resultProjectList = await _projectService.GetAllProjects(pageIndex, pageSize, businessId, areaId, listFieldId, investmentTargetCapital, name, status, currentUser);
                    return Ok(resultProjectList);
                }
            }           
            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            GetProjectDTO dto = new GetProjectDTO();

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {

                dto = await _projectService.GetProjectById(id);
                return Ok(dto);

            }
            else if (currentUser.roleId.Equals(currentUser.businessManagerRoleId))
            {
                if (currentUser.businessId.Equals(""))
                {
                    throw new System.UnauthorizedAccessException("You Don't Have Permission Perform This Action!!");
                }

                List<BusinessProjectDTO> projectList = await _projectService.GetBusinessProjectsToAuthor(Guid.Parse(currentUser.businessId));
                BusinessProjectDTO projectInfo = (BusinessProjectDTO)projectList
                    .Where(project => project.BusinessId.ToString().Equals(currentUser.businessId)).FirstOrDefault();

                if (projectInfo != null)
                {
                    dto = await _projectService.GetProjectById(id);
                    return Ok(dto);
                }
            }
            else if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                if (currentUser.businessId.Equals(""))
                {
                    throw new System.UnauthorizedAccessException("You Don't Have Permission Perform This Action!!");
                }

                List<BusinessProjectDTO> projectList = await _projectService.GetBusinessProjectsToAuthor(Guid.Parse(currentUser.businessId));
                BusinessProjectDTO projectInfo = (BusinessProjectDTO)projectList
                    .Where(project => project.ManagerId.ToString().Equals(currentUser.userId)).FirstOrDefault();

                if (projectInfo != null)
                {
                    dto = await _projectService.GetProjectById(id);
                    return Ok(dto);
                }

            }
            else if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                dto = await _projectService.GetProjectById(id);
                if (dto.status.Equals(ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString())
                    || dto.status.Equals(ProjectStatusEnum.WAITING_TO_ACTIVATE.ToString())
                    || dto.status.Equals(ProjectStatusEnum.ACTIVE.ToString())
                    || dto.status.Equals(ProjectStatusEnum.CLOSED.ToString()))
                {
                    return Ok(dto);
                }
                else
                {
                    throw new System.UnauthorizedAccessException("You Don't Have Permission Perform This Action!!");
                }
                //AllProjectDTO projectList = await _projectService.GetInvestedProjects(0, 0, currentUser);
                //bool investedCheck = false;
                //if (projectList.listOfProject.Count != 0)
                //{
                //    foreach (GetProjectDTO item in projectList.listOfProject)
                //    {
                //        if (id.ToString().Equals(item.id))
                //            investedCheck = true;
                //    }
                //    if (investedCheck)
                //    {
                //        dto = await _projectService.GetProjectById(id);
                //        return Ok(dto);
                //    }
                //    else
                //        throw new InvalidFieldException("You have not invested in the project have this Id!!!");
                //}
                //else
                //{
                //    throw new InvalidFieldException("You have not invested in the project have this Id!!!");
                //}



                //List<InvestorInvestmentDTO> investorList = await _investmentService.GetInvestmentByProjectIdForAuthor(id);
                //InvestorInvestmentDTO investor = (InvestorInvestmentDTO)investorList
                //    .Where(investor => investor.UserId.ToString().Equals(currentUser.userId)).FirstOrDefault();

                //if (investor != null)
                //{
                //    dto = await _projectService.GetProjectById(id);
                //    return Ok(dto);
                //}
            }
            else
            {
                dto = await _projectService.GetProjectById(id);
                if (dto.status.Equals(ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString()))
                {
                    return Ok(dto);
                }
                else
                {
                    throw new System.UnauthorizedAccessException("You Don't Have Permission Perform This Action!!");
                }

            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");

        }

        //GET INVESTED PROJECT
        [HttpGet]
        [Route("investedProject")]
        [Authorize]
        public async Task<IActionResult> GetInvestedProjects(int pageIndex, int pageSize)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _projectService.GetInvestedProjects(pageIndex, pageSize, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role INVESTOR can perform this action!!!");
        }

        //GET DETAILED INVESTED PROJECT
        [HttpGet]
        [Route("investedProject/{projectId}")]
        [Authorize]
        public async Task<IActionResult> GetInvestedProjectsDetail(string projectId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _projectService.GetInvestedProjectDetail(projectId, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role INVESTOR can perform this action!!!");
        }

        //UPDATE
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> UpdateProject([FromForm] UpdateProjectDTO projectDTO, Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                Data.Models.Entities.Business businessDTO = await _businessService.GetBusinessByProjectId(id);

                GetProjectDTO project = await _projectService.GetProjectById(id);

                if (businessDTO == null || project == null)
                {
                    throw new NotFoundException("no Project With This ID Found!!");
                } 
                else if(businessDTO.Id.ToString().Equals(currentUser.businessId))
                {
                    if(project.status.Equals(ProjectStatusEnum.DRAFT.ToString()) || project.status.Equals(ProjectStatusEnum.WAITING_FOR_APPROVAL.ToString()))
                    {
                        var result = await _projectService.UpdateProject(projectDTO, id, currentUser);
                        return Ok(result);
                    }
                }               
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
        }

        //UPDATE STATUS
        [HttpPut]
        [Route("status/{id},{status}")]
        [Authorize(Roles ="ADMIN, PROJECT_MANAGER")]
        public async Task<IActionResult> UpdateProjectStatus(Guid id, string status)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _projectService.UpdateProjectStatus(id, status, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or PROJECT_MANAGER can perform this action!!!");
        }

        //DELETE
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN, BUSINESS_MANAGER")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                GetProjectDTO project = await _projectService.GetProjectById(id);

                if (!project.status.Equals(ProjectStatusEnum.DRAFT.ToString()))
                {
                    var result = await _projectService.DeleteProjectById(id);
                    return Ok(result);
                }
            } else if (currentUser.roleId.Equals(currentUser.businessManagerRoleId))
            {
                GetProjectDTO project = await _projectService.GetProjectById(id);

                if (project.status.Equals(ProjectStatusEnum.DRAFT.ToString()) 
                    || project.status.Equals(ProjectStatusEnum.DENIED.ToString()) 
                    || project.status.Equals(ProjectStatusEnum.CLOSED.ToString()))
                {
                    var result = await _projectService.DeleteProjectById(id);
                    return Ok(result);
                }
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or BUSINESS_MANAGER can perform this action!!!");

        }

        //[HttpDelete]
        //[Authorize]
        //public async Task<IActionResult> ClearAllProjectData()
        //{
        //    //string userId = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value;

        //    //if (await _authenticateService.CheckRoleForAction(userId, RoleEnum.ADMIN.ToString()))
        //    //{
        //        var result = await _projectService.ClearAllProjectData();
        //        return Ok(result);
        //    //}
        //    //return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");

        //}
    }
}

 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Exceptions;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProject([FromForm] CreateUpdateProjectDTO projectDTO)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId) 
                && projectDTO.businessId.Equals(currentUser.businessId) 
                && projectDTO.managerId.Equals(currentUser.userId))
            {
                var result = await _projectService.CreateProject(projectDTO, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProjects(
            bool countOnly,
            int pageIndex,
            int pageSize,
            string businessId,
            string areaId,
            string fieldId,
            string name,
            string status
            )
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);
            if (countOnly)
            {
                var countResult = new ProjectCountDTO();
                countResult = await _projectService.CountProjects(businessId, areaId, fieldId, name, status, currentUser);
                return Ok(countResult);
            }
            else
            {
                var resultProjectList = new AllProjectDTO();
                resultProjectList = await _projectService.GetAllProjects(pageIndex, pageSize, businessId, areaId, fieldId, name, status, currentUser);
                return Ok(resultProjectList);
            }           
            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);
            GetProjectDTO dto = new GetProjectDTO();

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {

                dto = await _projectService.GetProjectById(id);
                return Ok(dto);

            } else if (currentUser.roleId.Equals(currentUser.businessManagerRoleId))
            {
                if (currentUser.businessId == null || currentUser.businessId == "")
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
            } else if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                if (currentUser.businessId == null || currentUser.businessId == "")
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

            } else if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                List<InvestorInvestmentDTO> investorList = await _investmentService.GetInvestmentByProjectIdForAuthor(id);
                InvestorInvestmentDTO investor = (InvestorInvestmentDTO)investorList
                    .Where(investor => investor.UserId.ToString().Equals(currentUser.userId)).FirstOrDefault();

                if (investor != null)
                {
                    dto = await _projectService.GetProjectById(id);
                    return Ok(dto);
                }
            }
            else
            {
                dto = await _projectService.GetProjectById(id);
                if (dto.status.Equals(ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString()))
                {
                    return Ok(dto);
                } else
                {
                    throw new System.UnauthorizedAccessException("You Don't Have Permission Perform This Action!!");
                }
                
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");

        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject([FromForm] CreateUpdateProjectDTO projectDTO, Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            if (currentUser.roleId.Equals(currentUser.businessManagerRoleId))
            {
                Data.Models.Entities.Business businessDTO = await _businessService.GetBusinessByProjectId(id);

                GetProjectDTO project = await _projectService.GetProjectById(id);

                if (businessDTO == null || project == null)
                {
                    throw new NotFoundException("no Project With This ID Found!!");
                } else if(businessDTO.Id.Equals(currentUser.businessId))
                {
                    if(project.status.Equals(ProjectStatusEnum.DRAFT.ToString()) || project.status.Equals(ProjectStatusEnum.WAITING_FOR_APPROVED.ToString()))
                    {
                        var result = await _projectService.UpdateProject(projectDTO, id);
                        return Ok(result);
                    }   
                }

                
            } else if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {

            }


            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

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

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");

        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> ClearAllProjectData()
        {
            //string userId = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value;

            //if (await _authenticateService.CheckRoleForAction(userId, RoleEnum.ADMIN.ToString()))
            //{
                var result = await _projectService.ClearAllProjectData();
                return Ok(result);
            //}
            //return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");

        }

        private async Task<ThisUserObj> GetThisUserInfo(HttpContext? httpContext)
        {
            ThisUserObj currentUser = new();
        
            var checkUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber);
            if(checkUser == null)
            {
                currentUser.userId = "";
                currentUser.email = "";
                currentUser.investorId = "";
            } else
            {
                currentUser.userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;
                currentUser.email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                currentUser.investorId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GroupSid).Value;
            }

            List<RoleDTO> roleList = await _roleService.GetAllRoles();
            GetUserDTO? userDTO = await _userService.GetUserByEmail(currentUser.email);
            if(userDTO != null)
            {
                if (userDTO.business == null)
                {
                    currentUser.roleId = "";
                    currentUser.businessId = "";

                }
                else
                {
                    currentUser.roleId = userDTO.role.id;
                    currentUser.businessId = userDTO.business.id;
                }
            }


            foreach (RoleDTO role in roleList)
            {
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(0)))
                {
                    currentUser.adminRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(3)))
                {
                    currentUser.investorRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(1)))
                {
                    currentUser.businessManagerRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(2)))
                {
                    currentUser.projectManagerRoleId = role.id;
                }
            }

            return currentUser;

        }
    }
}

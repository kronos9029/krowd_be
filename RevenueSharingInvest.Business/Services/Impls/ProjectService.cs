using AutoMapper;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;


        public ProjectService(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        //CREATE PROJECT
        public async Task<int> CreateProject(ProjectDTO newProjectDTO)
        {
            try
            {
                Project dto = _mapper.Map<Project>(newProjectDTO);
                int result = await _projectRepository.CreateProject(dto);
                //StageDTO stageDTO = newProjectDTO.stage;
                //List<PackageDTO> packageList = newProjectDTO.packageList;
                //List<PeriodRevenueDTO> periodRevenueList = newProjectDTO.periodRevenueList;
                //ProjectEntityDTO projectEntity = newProjectDTO.projectEntity
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<List<Project>> GetAllProjects()
        {
            List<Project> projectList = await _projectRepository.GetAllProjects();
            return projectList;
        }
    }
}

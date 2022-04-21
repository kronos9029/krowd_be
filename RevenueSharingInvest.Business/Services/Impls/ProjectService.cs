using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
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
        private readonly IStageRepository _stageRepository;
        private readonly IMapper _mapper;


        public ProjectService(IProjectRepository projectRepository, IStageRepository stageRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _stageRepository = stageRepository;
            _mapper = mapper;
        }

        //CREATE PROJECT
        public async Task<int> CreateProject(NewProjectDTO newProjectDTO)
        {
            int result;
            try
            {
                //Create project entity
                Project projectDTO = _mapper.Map<Project>(newProjectDTO.project);
                result = await _projectRepository.CreateProject(projectDTO);
                if(result == 0)
                    throw new CreateObjectException("Can not create Project Object!");

                //Create stage entities
                foreach (StageDTO stageDto in newProjectDTO.stageList)
                {
                    Stage stageDTO = _mapper.Map<Stage>(stageDto);
                    result = await _stageRepository.CreateStage(stageDTO);
                    if (result == 0)
                        throw new CreateObjectException("Can not create Stage Object!");
                }

                //Create package entities
                //foreach (PackageDTO packageDto in newProjectDTO.packageList)
                //{
                //    Package packageDTO = _mapper.Map<Package>(packageDto);
                //    result = await _stageRepository.CreateStage(stageDTO);
                //    if (result == 0)
                //        throw new CreateObjectException("Can not create Package Object!");
                //}
                

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

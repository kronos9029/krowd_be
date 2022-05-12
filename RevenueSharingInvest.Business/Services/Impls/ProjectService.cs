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

        //CREATE NEW PROJECT - Để dành chơi
        public async Task<int> CreateNewProject(NewProjectDTO newProjectDTO)
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

        //CREATE
        public async Task<int> CreateProject(ProjectDTO projectDTO)
        {
            int result;
            try
            {
                Project dto = _mapper.Map<Project>(projectDTO);
                result = await _projectRepository.CreateProject(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Project Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<ProjectDTO>> GetAllProjects()
        {
            List<Project> projectList = await _projectRepository.GetAllProjects();
            List<ProjectDTO> list = _mapper.Map<List<ProjectDTO>>(projectList);
            return list;
        }

        //UPDATE
        public async Task<int> UpdateProject(ProjectDTO projectDTO, Guid projectId)
        {
            int result;
            try
            {
                Project dto = _mapper.Map<Project>(projectDTO);
                result = await _projectRepository.UpdateProject(dto, projectId);
                if (result == 0)
                    throw new CreateObjectException("Can not update Project Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE

        public async Task<int> DeleteProjectById(Guid projectId)
        {
            int result;
            try
            {
                result = await _projectRepository.DeleteProjectById(projectId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete Project Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}

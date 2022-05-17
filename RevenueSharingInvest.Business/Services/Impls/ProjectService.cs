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
        private readonly IMapper _mapper;


        public ProjectService(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
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

        //GET ALL
        public async Task<List<ProjectDTO>> GetAllProjects()
        {
            List<Project> projectList = await _projectRepository.GetAllProjects();
            List<ProjectDTO> list = _mapper.Map<List<ProjectDTO>>(projectList);
            return list;
        }

        //GET BY ID
        public async Task<ProjectDTO> GetProjectById(Guid projectId)
        {
            ProjectDTO result;
            try
            {

                Project dto = await _projectRepository.GetProjectById(projectId);
                result = _mapper.Map<ProjectDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No Project Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
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
    }
}

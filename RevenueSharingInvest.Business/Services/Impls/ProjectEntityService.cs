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
    public class ProjectEntityService : IProjectEntityService
    {
        private readonly IProjectEntityRepository _projectEntityRepository;
        private readonly IMapper _mapper;


        public ProjectEntityService(IProjectEntityRepository projectEntityRepository, IMapper mapper)
        {
            _projectEntityRepository = projectEntityRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateProjectEntity(ProjectEntityDTO projectEntityDTO)
        {
            int result;
            try
            {
                ProjectEntity dto = _mapper.Map<ProjectEntity>(projectEntityDTO);
                result = await _projectEntityRepository.CreateProjectEntity(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create ProjectEntity Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteProjectEntityById(Guid projectEntityId)
        {
            int result;
            try
            {

                result = await _projectEntityRepository.DeleteProjectEntityById(projectEntityId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete ProjectEntity Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<ProjectEntityDTO>> GetAllProjectEntitys()
        {
            List<ProjectEntity> projectEntityList = await _projectEntityRepository.GetAllProjectEntitys();
            List<ProjectEntityDTO> list = _mapper.Map<List<ProjectEntityDTO>>(projectEntityList);
            return list;
        }

        //GET BY ID
        public async Task<ProjectEntityDTO> GetProjectEntityById(Guid projectEntityId)
        {
            ProjectEntityDTO result;
            try
            {

                ProjectEntity dto = await _projectEntityRepository.GetProjectEntityById(projectEntityId);
                result = _mapper.Map<ProjectEntityDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No ProjectEntity Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateProjectEntity(ProjectEntityDTO projectEntityDTO, Guid projectEntityId)
        {
            int result;
            try
            {
                ProjectEntity dto = _mapper.Map<ProjectEntity>(projectEntityDTO);
                result = await _projectEntityRepository.UpdateProjectEntity(dto, projectEntityId);
                if (result == 0)
                    throw new CreateObjectException("Can not update ProjectEntity Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}

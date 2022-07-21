using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
using RevenueSharingInvest.Business.Services.Common.Firebase;
using RevenueSharingInvest.Data.Models.Constants;
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
        private readonly IValidationService _validationService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;


        public ProjectEntityService(IProjectEntityRepository projectEntityRepository,
            IValidationService validationService,
            IFileUploadService fileUploadService,
            IMapper mapper)
        {
            _projectEntityRepository = projectEntityRepository;
            _validationService = validationService;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllProjectEntityData()
        {
            int result;
            try
            {
                result = await _projectEntityRepository.ClearAllProjectEntityData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateProjectEntity(CreateUpdateProjectEntityDTO projectEntityDTO)
        {
            IdDTO newId = new IdDTO();
            bool typeCheck = false;
            string typeErrorMessage = "";
            try
            {
                if (projectEntityDTO.projectId == null || !await _validationService.CheckUUIDFormat(projectEntityDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(projectEntityDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (!await _validationService.CheckText(projectEntityDTO.title))
                    throw new InvalidFieldException("Invalid title!!!");

                if (projectEntityDTO.content != null && (projectEntityDTO.content.Equals("string") || projectEntityDTO.content.Length == 0))
                    projectEntityDTO.content = null;

                if (projectEntityDTO.description != null && (projectEntityDTO.description.Equals("string") || projectEntityDTO.description.Length == 0))
                    projectEntityDTO.description = null;

                for (int type = 0; type < Enum.GetNames(typeof(ProjectEntityEnum)).Length; type++)
                {
                    if (projectEntityDTO.type.Equals(Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type)))
                        typeCheck = true;
                    typeErrorMessage = typeErrorMessage + " '" + Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type) + "' or";
                }
                typeErrorMessage = typeErrorMessage.Remove(typeErrorMessage.Length - 3);
                if (!typeCheck)
                    throw new InvalidFieldException("Type must be" + typeErrorMessage + " !!!");

                ProjectEntity entity = _mapper.Map<ProjectEntity>(projectEntityDTO);

                entity.Priority = (await _projectEntityRepository.CountProjectEntityByProjectIdAndType(entity.Id, entity.Type)) + 1;

                newId.id = await _projectEntityRepository.CreateProjectEntity(entity);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create ProjectEntity Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteProjectEntityById(Guid projectEntityId)
        {
            int result;
            ProjectEntity entity = new ProjectEntity();
            List<ProjectEntity> entityList = new List<ProjectEntity>();
            try
            {
                entity = await _projectEntityRepository.GetProjectEntityById(projectEntityId);
                result = await _projectEntityRepository.DeleteProjectEntityById(projectEntityId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete ProjectEntity Object!");
                entityList = await _projectEntityRepository.GetProjectEntityByProjectIdAndType(entity.ProjectId, entity.Type);
                for (int i = 0; i < entityList.Count; i++)
                {
                    if (await _projectEntityRepository.UpdateProjectEntityPriority(entityList[i].Id, i + 1) == 0)
                    {
                        for (int j = 0; j <= i ; j++)
                        {
                            await _projectEntityRepository.UpdateProjectEntityPriority(entityList[j].Id, entityList[j].Priority);
                        }
                        throw new UpdateObjectException("Can not update ProjectEntity priority!");
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<GetProjectEntityDTO>> GetAllProjectEntities(int pageIndex, int pageSize)
        {
            try
            {
                List<ProjectEntity> projectEntityList = await _projectEntityRepository.GetAllProjectEntities(pageIndex, pageSize);
                List<GetProjectEntityDTO> list = _mapper.Map<List<GetProjectEntityDTO>>(projectEntityList);

                foreach (GetProjectEntityDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);
                }

                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetProjectEntityDTO> GetProjectEntityById(Guid projectEntityId)
        {
            GetProjectEntityDTO result;
            try
            {

                ProjectEntity dto = await _projectEntityRepository.GetProjectEntityById(projectEntityId);
                result = _mapper.Map<GetProjectEntityDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No ProjectEntity Object Found!");

                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateProjectEntity(CreateUpdateProjectEntityDTO projectEntityDTO, Guid projectEntityId)
        {
            int result;
            bool typeCheck = false;
            string typeErrorMessage = "";
            try
            {
                if (projectEntityDTO.projectId == null || !await _validationService.CheckUUIDFormat(projectEntityDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(projectEntityDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (!await _validationService.CheckText(projectEntityDTO.title))
                    throw new InvalidFieldException("Invalid title!!!");

                if (projectEntityDTO.content != null && (projectEntityDTO.content.Equals("string") || projectEntityDTO.content.Length == 0))
                    projectEntityDTO.content = null;

                if (projectEntityDTO.description != null && (projectEntityDTO.description.Equals("string") || projectEntityDTO.description.Length == 0))
                    projectEntityDTO.description = null;

                for (int type = 0; type < Enum.GetNames(typeof(ProjectEntityEnum)).Length; type++)
                {
                    if (projectEntityDTO.type.Equals(Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type)))
                        typeCheck = true;
                    typeErrorMessage = typeErrorMessage + " '" + Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type) + "' or";
                }
                typeErrorMessage = typeErrorMessage.Remove(typeErrorMessage.Length - 3);
                if (!typeCheck)
                    throw new InvalidFieldException(typeErrorMessage + " !!!");

                ProjectEntity entity = _mapper.Map<ProjectEntity>(projectEntityDTO);

                result = await _projectEntityRepository.UpdateProjectEntity(entity, projectEntityId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update ProjectEntity Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> UpdateProjectEntityPriority(List<string> idList)
        {
            ProjectEntity entity = new ProjectEntity();
            List<ProjectEntity> entityList = new List<ProjectEntity>();
            try
            {
                for (int i = 0; i < idList.Count; i++)
                {
                    entity = await _projectEntityRepository.GetProjectEntityById(Guid.Parse(idList[i]));
                    if (entity == null)
                        throw new NotFoundException("Id " + idList[i] + " ProjectEntity Object not found!");
                    entityList.Add(entity);

                    if (await _projectEntityRepository.UpdateProjectEntityPriority(Guid.Parse(idList[i]), i + 1) == 0)
                    {
                        foreach (ProjectEntity item in entityList)
                        {
                            await _projectEntityRepository.UpdateProjectEntityPriority(item.Id, item.Priority);
                        }
                        throw new UpdateObjectException("Can not update ProjectEntity Object!");
                    }
                }
                return idList.Count;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

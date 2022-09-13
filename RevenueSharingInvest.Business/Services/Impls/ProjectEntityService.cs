using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
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
        private readonly IProjectRepository _projectRepository;
        private readonly IValidationService _validationService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;


        public ProjectEntityService(IProjectEntityRepository projectEntityRepository,
            IProjectRepository projectRepository,
            IValidationService validationService,
            IFileUploadService fileUploadService,
            IMapper mapper)
        {
            _projectEntityRepository = projectEntityRepository;
            _projectRepository = projectRepository;
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
        public async Task<IdDTO> CreateProjectEntity(CreateProjectEntityDTO projectEntityDTO, ThisUserObj currentUser)
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

                //Kiểm tra projectId có thuộc về business của PM không
                Project project = await _projectRepository.GetProjectById(Guid.Parse(projectEntityDTO.projectId));
                if (!project.BusinessId.ToString().Equals(currentUser.businessId))
                {
                    throw new NotFoundException("This projectId is not belong to your's Business!!!");
                }
                //

                if (!await _validationService.CheckText(projectEntityDTO.title))
                    throw new InvalidFieldException("Invalid title!!!");

                if (projectEntityDTO.content != null && (projectEntityDTO.content.Equals("string") || projectEntityDTO.content.Length == 0))
                    projectEntityDTO.content = null;

                if (projectEntityDTO.link != null && (projectEntityDTO.link.Equals("string") || projectEntityDTO.link.Length == 0))
                    projectEntityDTO.link = null;

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

                entity.Priority = (await _projectEntityRepository.CountProjectEntityByProjectIdAndType(entity.ProjectId, entity.Type)) + 1;
                entity.CreateBy = Guid.Parse(currentUser.userId);
                entity.UpdateBy = Guid.Parse(currentUser.userId);

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
                if (entity == null)
                    throw new DeleteObjectException("No ProjectEntity Object Founded!");
                result = await _projectEntityRepository.DeleteProjectEntityById(projectEntityId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete ProjectEntity Object!");

                entityList = await _projectEntityRepository.GetProjectEntityByProjectIdAndType(entity.ProjectId, entity.Type);

                for (int i = 0; i < entityList.Count; i++)
                {
                    if (await _projectEntityRepository.UpdateProjectEntityPriority(entityList[i].Id, i + 1, null) == 0)
                    {
                        for (int j = 0; j <= i ; j++)
                        {
                            await _projectEntityRepository.UpdateProjectEntityPriority(entityList[j].Id, entityList[j].Priority, null);
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
        public async Task<List<GetProjectEntityDTO>> GetProjectEntityByProjectIdAndType(Guid projectId, string type, ThisUserObj currentUser)
        {
            bool typeCheck = false;
            string typeErrorMessage = "";

            try
            {
                //Kiểm tra projectId có thuộc về business của người xem có role BuM hay PM không
                Project project = await _projectRepository.GetProjectById(projectId);
                if ((currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")) || currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                    && !project.BusinessId.ToString().Equals(currentUser.businessId))
                {
                    throw new NotFoundException("This projectId is not belong to your's Business!!!");
                }
                //

                if (type != null)
                {
                    for (int check = 0; check < Enum.GetNames(typeof(ProjectEntityEnum)).Length; check++)
                    {
                        if (type.Equals(Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(check)))
                            typeCheck = true;
                        typeErrorMessage = typeErrorMessage + " '" + Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(check) + "' or";
                    }
                    typeErrorMessage = typeErrorMessage.Remove(typeErrorMessage.Length - 3);
                    if (!typeCheck)
                        throw new InvalidFieldException("Type must be" + typeErrorMessage + " !!!");
                }

                List<ProjectEntity> projectEntityList = await _projectEntityRepository.GetProjectEntityByProjectIdAndType(projectId, type);
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
        public async Task<GetProjectEntityDTO> GetProjectEntityById(Guid projectEntityId, ThisUserObj currentUser)
        {
            GetProjectEntityDTO result;
            bool typeCheck = false;
            string typeErrorMessage = "";

            try
            {
                ProjectEntity projectEntity = await _projectEntityRepository.GetProjectEntityById(projectEntityId);
                if (projectEntity == null)
                    throw new NotFoundException("No ProjectEntity Object Found!");

                //Kiểm tra projectId có thuộc về business của người xem có role BuM hay PM không
                Project project = await _projectRepository.GetProjectById(projectEntity.ProjectId);
                if ((currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")) || currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                    && !project.BusinessId.ToString().Equals(currentUser.businessId))
                {
                    throw new NotFoundException("This projectId is not belong to your's Business!!!");
                }
                //

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
        public async Task<int> UpdateProjectEntity(UpdateProjectEntityDTO projectEntityDTO, Guid projectEntityId, ThisUserObj currentUser)
        {
            int result;
            bool typeCheck = false;
            string typeErrorMessage = "";
            try
            {
                ProjectEntity projectEntity = await _projectEntityRepository.GetProjectEntityById(projectEntityId);
                Project project = await _projectRepository.GetProjectById(projectEntity.ProjectId);

                if (!project.ManagerId.ToString().Equals(currentUser.userId))
                    throw new InvalidFieldException("This projectEntityId is not belong to your Project!!!");

                if (projectEntityDTO.title != null)
                {
                    if (!await _validationService.CheckText(projectEntityDTO.title))
                        throw new InvalidFieldException("Invalid title!!!");
                }
                
                if (projectEntityDTO.content != null && (projectEntityDTO.content.Equals("string") || projectEntityDTO.content.Length == 0))
                    projectEntityDTO.content = null;

                if (projectEntityDTO.link != null && (projectEntityDTO.link.Equals("string") || projectEntityDTO.link.Length == 0))
                    projectEntityDTO.link = null;

                if (projectEntityDTO.description != null && (projectEntityDTO.description.Equals("string") || projectEntityDTO.description.Length == 0))
                    projectEntityDTO.description = null;

                if (projectEntityDTO.type != null)
                {
                    for (int type = 0; type < Enum.GetNames(typeof(ProjectEntityEnum)).Length; type++)
                    {
                        if (projectEntityDTO.type.Equals(Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type)))
                            typeCheck = true;
                        typeErrorMessage = typeErrorMessage + " '" + Enum.GetNames(typeof(ProjectEntityEnum)).ElementAt(type) + "' or";
                    }
                    typeErrorMessage = typeErrorMessage.Remove(typeErrorMessage.Length - 3);
                    if (!typeCheck)
                        throw new InvalidFieldException("Type must be" + typeErrorMessage + " !!!");
                }                

                ProjectEntity entity = _mapper.Map<ProjectEntity>(projectEntityDTO);
                entity.UpdateBy = Guid.Parse(currentUser.userId);

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

        public async Task<int> UpdateProjectEntityPriority(List<ProjectEntityUpdateDTO> idList, ThisUserObj currentUser)
        {
            ProjectEntity entity = new ProjectEntity();
            List<ProjectEntity> entityList = new List<ProjectEntity>();
            Guid projectIdCheck;
            string typeCheck;
            try
            {
                projectIdCheck = (await _projectEntityRepository.GetProjectEntityById(Guid.Parse(idList[0].id))).ProjectId;
                typeCheck = (await _projectEntityRepository.GetProjectEntityById(Guid.Parse(idList[0].id))).Type;
                foreach (ProjectEntityUpdateDTO item in idList)
                {
                    entity = await _projectEntityRepository.GetProjectEntityById(Guid.Parse(item.id));
                    if (entity == null)
                        throw new NotFoundException("Id " + item.id + " ProjectEntity Object not found!");
                    if (!entity.ProjectId.Equals(projectIdCheck))
                        throw new InvalidFieldException("The ProjectEntity objects do not have the same ProjectId !");
                    if (!entity.Type.Equals(typeCheck))
                        throw new InvalidFieldException("The ProjectEntity objects do not have the same Type !");
                    entityList.Add(entity);
                }

                foreach (ProjectEntityUpdateDTO item in idList)
                {
                    if (await _projectEntityRepository.UpdateProjectEntityPriority(Guid.Parse(item.id), item.priority, Guid.Parse(currentUser.userId)) == 0)
                    {
                        foreach (ProjectEntity i in entityList)
                        {
                            await _projectEntityRepository.UpdateProjectEntityPriority(i.Id, i.Priority, i.UpdateBy);
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

using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
using RevenueSharingInvest.Business.Services.Common.Firebase;
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
        public async Task<IdDTO> CreateProjectEntity(ProjectEntityDTO projectEntityDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (projectEntityDTO.projectId == null || !await _validationService.CheckUUIDFormat(projectEntityDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(projectEntityDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (projectEntityDTO.image != null && (projectEntityDTO.image.Equals("string") || projectEntityDTO.image.Length == 0))
                    projectEntityDTO.image = null;

                if (!await _validationService.CheckText(projectEntityDTO.title))
                    throw new InvalidFieldException("Invalid title!!!");

                if (projectEntityDTO.description != null && (projectEntityDTO.description.Equals("string") || projectEntityDTO.description.Length == 0))
                    projectEntityDTO.description = null;

                if (!await _validationService.CheckText(projectEntityDTO.type) || (!projectEntityDTO.type.Equals("UPDATE") && !projectEntityDTO.type.Equals("HIGHLIGHT")))
                    throw new InvalidFieldException("type must be 'UPDATE' or 'HIGHLIGHT'!!!");

                if (projectEntityDTO.createBy != null && projectEntityDTO.createBy.Length >= 0)
                {
                    if (projectEntityDTO.createBy.Equals("string"))
                        projectEntityDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(projectEntityDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (projectEntityDTO.updateBy != null && projectEntityDTO.updateBy.Length >= 0)
                {
                    if (projectEntityDTO.updateBy.Equals("string"))
                        projectEntityDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(projectEntityDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                projectEntityDTO.isDeleted = false;

                ProjectEntity entity = _mapper.Map<ProjectEntity>(projectEntityDTO);

                if(projectEntityDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseProjectEntity(projectEntityDTO.image, projectEntityDTO.createBy);
                }

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
            try
            {

                result = await _projectEntityRepository.DeleteProjectEntityById(projectEntityId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete ProjectEntity Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<ProjectEntityDTO>> GetAllProjectEntities(int pageIndex, int pageSize)
        {
            try
            {
                List<ProjectEntity> projectEntityList = await _projectEntityRepository.GetAllProjectEntities(pageIndex, pageSize);
                List<ProjectEntityDTO> list = _mapper.Map<List<ProjectEntityDTO>>(projectEntityList);

                foreach (ProjectEntityDTO item in list)
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
        public async Task<ProjectEntityDTO> GetProjectEntityById(Guid projectEntityId)
        {
            ProjectEntityDTO result;
            try
            {

                ProjectEntity dto = await _projectEntityRepository.GetProjectEntityById(projectEntityId);
                result = _mapper.Map<ProjectEntityDTO>(dto);
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
        public async Task<int> UpdateProjectEntity(ProjectEntityDTO projectEntityDTO, Guid projectEntityId)
        {
            int result;
            try
            {
                if (projectEntityDTO.projectId == null || !await _validationService.CheckUUIDFormat(projectEntityDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(projectEntityDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (projectEntityDTO.image != null && (projectEntityDTO.image.Equals("string") || projectEntityDTO.image.Length == 0))
                    projectEntityDTO.image = null;

                if (!await _validationService.CheckText(projectEntityDTO.title))
                    throw new InvalidFieldException("Invalid title!!!");

                if (projectEntityDTO.description != null && (projectEntityDTO.description.Equals("string") || projectEntityDTO.description.Length == 0))
                    projectEntityDTO.description = null;

                if (!await _validationService.CheckText(projectEntityDTO.type) || (!projectEntityDTO.type.Equals("UPDATE") && !projectEntityDTO.type.Equals("HIGHLIGHT")))
                    throw new InvalidFieldException("type must be 'UPDATE' or 'HIGHLIGHT'!!!");

                if (projectEntityDTO.createBy != null && projectEntityDTO.createBy.Length >= 0)
                {
                    if (projectEntityDTO.createBy.Equals("string"))
                        projectEntityDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(projectEntityDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (projectEntityDTO.updateBy != null && projectEntityDTO.updateBy.Length >= 0)
                {
                    if (projectEntityDTO.updateBy.Equals("string"))
                        projectEntityDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(projectEntityDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                ProjectEntity entity = _mapper.Map<ProjectEntity>(projectEntityDTO);

                if (projectEntityDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseProjectEntity(projectEntityDTO.image, projectEntityDTO.updateBy);
                }

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
    }
}

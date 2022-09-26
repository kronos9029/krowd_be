using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class ProjectWalletService : IProjectWalletService
    {
        private readonly IProjectWalletRepository _projectWalletRepository;
        private readonly IWalletTypeRepository _walletTypeRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public ProjectWalletService(IProjectWalletRepository projectWalletRepository, IWalletTypeRepository walletTypeRepository, IValidationService validationService, IMapper mapper)
        {
            _projectWalletRepository = projectWalletRepository;
            _walletTypeRepository = walletTypeRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        //public async Task<int> ClearAllProjectWalletData()
        //{
        //    int result;
        //    try
        //    {
        //        result = await _projectWalletRepository.ClearAllProjectWalletData();
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //CREATE
        //public async Task<IdDTO> CreateProjectWallet(ProjectWalletDTO projectWalletDTO)
        //{
        //    IdDTO newId = new IdDTO();
        //    try
        //    {
        //        if (projectWalletDTO.projectId == null || !await _validationService.CheckUUIDFormat(projectWalletDTO.projectId))
        //            throw new InvalidFieldException("Invalid projectId!!!");

        //        if (!await _validationService.CheckExistenceId("Project", Guid.Parse(projectWalletDTO.projectId)))
        //            throw new NotFoundException("This projectId is not existed!!!");

        //        if (projectWalletDTO.walletTypeId == null || !await _validationService.CheckUUIDFormat(projectWalletDTO.walletTypeId))
        //            throw new InvalidFieldException("Invalid walletTypeId!!!");

        //        if (!await _validationService.CheckExistenceId("WalletType", Guid.Parse(projectWalletDTO.walletTypeId)))
        //            throw new NotFoundException("This walletTypeId is not existed!!!");

        //        if (projectWalletDTO.createBy != null && projectWalletDTO.createBy.Length >= 0)
        //        {
        //            if (projectWalletDTO.createBy.Equals("string"))
        //                projectWalletDTO.createBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(projectWalletDTO.createBy))
        //                throw new InvalidFieldException("Invalid createBy!!!");
        //        }

        //        if (projectWalletDTO.updateBy != null && projectWalletDTO.updateBy.Length >= 0)
        //        {
        //            if (projectWalletDTO.updateBy.Equals("string"))
        //                projectWalletDTO.updateBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(projectWalletDTO.updateBy))
        //                throw new InvalidFieldException("Invalid updateBy!!!");
        //        }

        //        projectWalletDTO.isDeleted = false;

        //        ProjectWallet dto = _mapper.Map<ProjectWallet>(projectWalletDTO);
        //        newId.id = await _projectWalletRepository.CreateProjectWallet(dto);
        //        if (newId.id.Equals(""))
        //            throw new CreateObjectException("Can not create ProjectWallet Object!");
        //        return newId;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //DELETE
        //public async Task<int> DeleteProjectWalletById(Guid projectWalletId)
        //{
        //    int result;
        //    try
        //    {
        //        result = await _projectWalletRepository.DeleteProjectWalletById(projectWalletId);
        //        if (result == 0)
        //            throw new DeleteObjectException("Can not delete ProjectWallet Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //GET ALL
        public async Task<UserWalletsDTO> GetAllProjectWallets(ThisUserObj currentUser)
        {
            try
            {
                UserWalletsDTO result = new UserWalletsDTO();
                result.listOfProjectWallet = new List<GetProjectWalletDTO>();
                List<ProjectWallet> projectWalletList = await _projectWalletRepository.GetProjectWalletsByProjectManagerId(Guid.Parse(currentUser.userId));
                List<MappedProjectWalletDTO> list = _mapper.Map<List<MappedProjectWalletDTO>>(projectWalletList);
                GetProjectWalletDTO dto = new GetProjectWalletDTO();

                foreach (MappedProjectWalletDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    dto = _mapper.Map<GetProjectWalletDTO>(item);
                    dto.walletType = _mapper.Map<GetWalletTypeForWalletDTO>(await _walletTypeRepository.GetWalletTypeById(Guid.Parse(item.walletTypeId)));

                    result.totalAsset += (float)item.balance;
                    result.listOfProjectWallet.Add(dto);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        //public async Task<ProjectWalletDTO> GetProjectWalletById(Guid projectWalletId)
        //{
        //    ProjectWalletDTO result;
        //    try
        //    {

        //        ProjectWallet dto = await _projectWalletRepository.GetProjectWalletById(projectWalletId);
        //        result = _mapper.Map<ProjectWalletDTO>(dto);
        //        if (result == null)
        //            throw new NotFoundException("No ProjectWallet Object Found!");

        //        result.createDate = await _validationService.FormatDateOutput(result.createDate);
        //        result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //UPDATE
        //public async Task<int> UpdateProjectWallet(ProjectWalletDTO projectWalletDTO, Guid projectWalletId)
        //{
        //    int result;
        //    try
        //    {
        //        if (projectWalletDTO.projectId == null || !await _validationService.CheckUUIDFormat(projectWalletDTO.projectId))
        //            throw new InvalidFieldException("Invalid projectId!!!");

        //        if (!await _validationService.CheckExistenceId("Project", Guid.Parse(projectWalletDTO.projectId)))
        //            throw new NotFoundException("This projectId is not existed!!!");

        //        if (projectWalletDTO.walletTypeId == null || !await _validationService.CheckUUIDFormat(projectWalletDTO.walletTypeId))
        //            throw new InvalidFieldException("Invalid walletTypeId!!!");

        //        if (!await _validationService.CheckExistenceId("WalletType", Guid.Parse(projectWalletDTO.walletTypeId)))
        //            throw new NotFoundException("This walletTypeId is not existed!!!");

        //        if (projectWalletDTO.createBy != null && projectWalletDTO.createBy.Length >= 0)
        //        {
        //            if (projectWalletDTO.createBy.Equals("string"))
        //                projectWalletDTO.createBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(projectWalletDTO.createBy))
        //                throw new InvalidFieldException("Invalid createBy!!!");
        //        }

        //        if (projectWalletDTO.updateBy != null && projectWalletDTO.updateBy.Length >= 0)
        //        {
        //            if (projectWalletDTO.updateBy.Equals("string"))
        //                projectWalletDTO.updateBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(projectWalletDTO.updateBy))
        //                throw new InvalidFieldException("Invalid updateBy!!!");
        //        }

        //        ProjectWallet dto = _mapper.Map<ProjectWallet>(projectWalletDTO);
        //        result = await _projectWalletRepository.UpdateProjectWallet(dto, projectWalletId);
        //        if (result == 0)
        //            throw new UpdateObjectException("Can not update ProjectWallet Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}
    }
}

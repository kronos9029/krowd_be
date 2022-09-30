using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
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
                LoggerService.Logger(e.ToString());
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
    }
}

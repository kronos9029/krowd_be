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
    public class InvestorWalletService : IInvestorWalletService
    {
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IWalletTypeRepository _walletTypeRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public InvestorWalletService(IInvestorWalletRepository investorWalletRepository, IWalletTypeRepository walletTypeRepository, IValidationService validationService, IMapper mapper)
        {
            _investorWalletRepository = investorWalletRepository;
            _walletTypeRepository = walletTypeRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        //public async Task<int> ClearAllInvestorWalletData()
        //{
        //    int result;
        //    try
        //    {
        //        result = await _investorWalletRepository.ClearAllInvestorWalletData();
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //CREATE
        //public async Task<IdDTO> CreateInvestorWallet(InvestorWalletDTO investorWalletDTO)
        //{
        //    IdDTO newId = new IdDTO();
        //    try
        //    {
        //        if (investorWalletDTO.investorId == null || !await _validationService.CheckUUIDFormat(investorWalletDTO.investorId))
        //            throw new InvalidFieldException("Invalid investorId!!!");

        //        if (!await _validationService.CheckExistenceId("Investor", Guid.Parse(investorWalletDTO.investorId)))
        //            throw new NotFoundException("This investorId is not existed!!!");

        //        if (investorWalletDTO.walletTypeId == null || !await _validationService.CheckUUIDFormat(investorWalletDTO.walletTypeId))
        //            throw new InvalidFieldException("Invalid walletTypeId!!!");

        //        if (!await _validationService.CheckExistenceId("WalletType", Guid.Parse(investorWalletDTO.walletTypeId)))
        //            throw new NotFoundException("This walletTypeId is not existed!!!");

        //        if (investorWalletDTO.createBy != null && investorWalletDTO.createBy.Length >= 0)
        //        {
        //            if (investorWalletDTO.createBy.Equals("string"))
        //                investorWalletDTO.createBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investorWalletDTO.createBy))
        //                throw new InvalidFieldException("Invalid createBy!!!");
        //        }

        //        if (investorWalletDTO.updateBy != null && investorWalletDTO.updateBy.Length >= 0)
        //        {
        //            if (investorWalletDTO.updateBy.Equals("string"))
        //                investorWalletDTO.updateBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investorWalletDTO.updateBy))
        //                throw new InvalidFieldException("Invalid updateBy!!!");
        //        }

        //        investorWalletDTO.isDeleted = false;

        //        InvestorWallet dto = _mapper.Map<InvestorWallet>(investorWalletDTO);
        //        newId.id = await _investorWalletRepository.CreateInvestorWallet(dto);
        //        if (newId.id.Equals(""))
        //            throw new CreateObjectException("Can not create InvestorWallet Object!");
        //        return newId;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //DELETE
        //public async Task<int> DeleteInvestorWalletById(Guid investorWalletId)
        //{
        //    int result;
        //    try
        //    {
        //        result = await _investorWalletRepository.DeleteInvestorWalletById(investorWalletId);
        //        if (result == 0)
        //            throw new DeleteObjectException("Can not delete InvestorWallet Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //GET ALL
        public async Task<UserWalletsDTO> GetAllInvestorWallets(ThisUserObj currentUser)
        {
            try
            {
                UserWalletsDTO result = new UserWalletsDTO();
                result.listOfInvestorWallet = new List<GetInvestorWalletDTO>();
                List<InvestorWallet> investorWalletList = await _investorWalletRepository.GetInvestorWalletsByInvestorId(Guid.Parse(currentUser.investorId));
                List<MappedInvestorWalletDTO> list = _mapper.Map<List<MappedInvestorWalletDTO>>(investorWalletList);
                GetInvestorWalletDTO dto = new GetInvestorWalletDTO();

                foreach (MappedInvestorWalletDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    dto = _mapper.Map<GetInvestorWalletDTO>(item);
                    dto.walletType = _mapper.Map<GetWalletTypeForWalletDTO>(await _walletTypeRepository.GetWalletTypeById(Guid.Parse(item.walletTypeId)));

                    result.totalAsset += (float)item.balance;
                    result.listOfInvestorWallet.Add(dto);
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
        //public async Task<InvestorWalletDTO> GetInvestorWalletById(Guid investorWalletId)
        //{
        //    InvestorWalletDTO result;
        //    try
        //    {
        //        InvestorWallet dto = await _investorWalletRepository.GetInvestorWalletById(investorWalletId);
        //        result = _mapper.Map<InvestorWalletDTO>(dto);
        //        if (result == null)
        //            throw new NotFoundException("No InvestorWallet Object Found!");

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
        //public async Task<int> UpdateInvestorWallet(InvestorWalletDTO investorWalletDTO, Guid investorWalletId)
        //{
        //    int result;
        //    try
        //    {
        //        if (investorWalletDTO.investorId == null || !await _validationService.CheckUUIDFormat(investorWalletDTO.investorId))
        //            throw new InvalidFieldException("Invalid investorId!!!");

        //        if (!await _validationService.CheckExistenceId("Investor", Guid.Parse(investorWalletDTO.investorId)))
        //            throw new NotFoundException("This investorId is not existed!!!");

        //        if (investorWalletDTO.walletTypeId == null || !await _validationService.CheckUUIDFormat(investorWalletDTO.walletTypeId))
        //            throw new InvalidFieldException("Invalid walletTypeId!!!");

        //        if (!await _validationService.CheckExistenceId("WalletType", Guid.Parse(investorWalletDTO.walletTypeId)))
        //            throw new NotFoundException("This walletTypeId is not existed!!!");

        //        if (investorWalletDTO.createBy != null && investorWalletDTO.createBy.Length >= 0)
        //        {
        //            if (investorWalletDTO.createBy.Equals("string"))
        //                investorWalletDTO.createBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investorWalletDTO.createBy))
        //                throw new InvalidFieldException("Invalid createBy!!!");
        //        }

        //        if (investorWalletDTO.updateBy != null && investorWalletDTO.updateBy.Length >= 0)
        //        {
        //            if (investorWalletDTO.updateBy.Equals("string"))
        //                investorWalletDTO.updateBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investorWalletDTO.updateBy))
        //                throw new InvalidFieldException("Invalid updateBy!!!");
        //        }

        //        InvestorWallet dto = _mapper.Map<InvestorWallet>(investorWalletDTO);
        //        result = await _investorWalletRepository.UpdateInvestorWallet(dto, investorWalletId);
        //        if (result == 0)
        //            throw new CreateObjectException("Can not update InvestorWallet Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}
    }
}

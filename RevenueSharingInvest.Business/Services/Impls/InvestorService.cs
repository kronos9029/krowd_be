using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
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
    public class InvestorService : IInvestorService
    {
        private readonly IInvestorRepository _investorRepository;
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public InvestorService(IInvestorRepository investorRepository, IInvestorWalletRepository investorWalletRepository, IValidationService validationService, IMapper mapper)
        {
            _investorRepository = investorRepository;
            _investorWalletRepository = investorWalletRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        //public async Task<int> ClearAllInvestorData()
        //{
        //    int result;
        //    try
        //    {
        //        result = await _investorRepository.ClearAllInvestorData();
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //CREATE
        //public async Task<IdDTO> CreateInvestor(InvestorDTO investorDTO)
        //{
        //    IdDTO newId = new IdDTO();
        //    try
        //    {
        //        if (investorDTO.userID == null || !await _validationService.CheckUUIDFormat(investorDTO.userID))
        //            throw new InvalidFieldException("Invalid userId!!!");

        //        if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(investorDTO.userID)))
        //            throw new NotFoundException("This userId is not existed!!!");

        //        if (investorDTO.investorTypeID == null || !await _validationService.CheckUUIDFormat(investorDTO.investorTypeID))
        //            throw new InvalidFieldException("Invalid investorTypeID!!!");

        //        if (!await _validationService.CheckExistenceId("InvestorType", Guid.Parse(investorDTO.investorTypeID)))
        //            throw new NotFoundException("This investorTypeID is not existed!!!");

        //        if (investorDTO.status < 0 || investorDTO.status > 2)
        //            throw new InvalidFieldException("Status must be 0(ACTIVE) or 1(INACTIVE) or 2(BLOCKED)!!!");

        //        if (investorDTO.createBy != null && investorDTO.createBy.Length >= 0)
        //        {
        //            if (investorDTO.createBy.Equals("string"))
        //                investorDTO.createBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investorDTO.createBy))
        //                throw new InvalidFieldException("Invalid createBy!!!");
        //        }

        //        if (investorDTO.updateBy != null && investorDTO.updateBy.Length >= 0)
        //        {
        //            if (investorDTO.updateBy.Equals("string"))
        //                investorDTO.updateBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investorDTO.updateBy))
        //                throw new InvalidFieldException("Invalid updateBy!!!");
        //        }

        //        investorDTO.isDeleted = false;

        //        Investor dto = _mapper.Map<Investor>(investorDTO);
        //        newId.id = await _investorRepository.CreateInvestor(dto);
        //        if (newId.id.Equals(""))
        //            throw new CreateObjectException("Can not create Investor Object!");
        //        return newId;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}
        
        //DELETE
        //public async Task<int> DeleteInvestorById(Guid investorId)
        //{
        //    int result;
        //    try
        //    {

        //        result = await _investorRepository.DeleteInvestorById(investorId);
        //        if (result == 0)
        //            throw new DeleteObjectException("Can not delete Investor Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}


        //GET ALL
        //public async Task<AllInvestorDTO> GetAllInvestors(int pageIndex, int pageSize, string status, string name, string orderBy, string order, ThisUserObj currentUser)
        //{
        //    AllInvestorDTO result = new AllInvestorDTO();
        //    result.listOfInvestor = new List<GetInvestorDTO>();
        //    List<Investor> listEntity = new List<Investor>();

        //    string orderByErrorMessage = "";
        //    bool checkOrderError = false;
        //    string orderErrorMessage = "";
        //    bool statusCheck = false;
        //    string statusErrorMessage = "";
        //    try
        //    {
        //        if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
        //        {
        //            int[] statusNum = { 0, 1, 2 };

        //            if (status != null)
        //            {
        //                foreach (int item in statusNum)
        //                {
        //                    if (status.Equals(Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item)))
        //                        statusCheck = true;
        //                    statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item) + "' or";
        //                }
        //                statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
        //                if (!statusCheck)
        //                    throw new InvalidFieldException("ADMIN can view Investors with status" + statusErrorMessage + " !!!");
        //            }
        //        }
        //        else if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
        //        {
        //            int[] statusNum = { 0, 1, 2 };

        //            if (status != null)
        //            {
        //                foreach (int item in statusNum)
        //                {
        //                    if (status.Equals(Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item)))
        //                        statusCheck = true;
        //                    statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item) + "' or";
        //                }
        //                statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
        //                if (!statusCheck)
        //                    throw new InvalidFieldException("BUSINESS_MANAGER can view Investors with status" + statusErrorMessage + " !!!");
        //            }
        //        }
        //        else if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
        //        {
        //            int[] statusNum = { 0, 1, 2 };

        //            if (status != null)
        //            {
        //                foreach (int item in statusNum)
        //                {
        //                    if (status.Equals(Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item)))
        //                        statusCheck = true;
        //                    statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item) + "' or";
        //                }
        //                statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
        //                if (!statusCheck)
        //                    throw new InvalidFieldException("PROJECT_MANAGER can view Investors with status" + statusErrorMessage + " !!!");
        //            }
        //        }

        //        List<Investor> investorList = await _investorRepository.GetAllInvestors(pageIndex, pageSize);
        //        List<InvestorDTO> list = _mapper.Map<List<InvestorDTO>>(investorList);

        //        foreach (InvestorDTO item in list)
        //        {
        //            item.createDate = await _validationService.FormatDateOutput(item.createDate);
        //            item.updateDate = await _validationService.FormatDateOutput(item.updateDate);
        //        }

        //        return list;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //GET BY ID
        //public async Task<GetInvestorDTO> GetInvestorById(Guid investorId)
        //{
        //    InvestorDTO result;
        //    try
        //    {

        //        Investor dto = await _investorRepository.GetInvestorById(investorId);
        //        result = _mapper.Map<InvestorDTO>(dto);
        //        if (result == null)
        //            throw new NotFoundException("No Investor Object Found!");

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
        //public async Task<int> UpdateInvestor(InvestorDTO investorDTO, Guid investorId)
        //{
        //    int result;
        //    try
        //    {
        //        if (investorDTO.userID == null || !await _validationService.CheckUUIDFormat(investorDTO.userID))
        //            throw new InvalidFieldException("Invalid userId!!!");

        //        if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(investorDTO.userID)))
        //            throw new NotFoundException("This userId is not existed!!!");

        //        if (investorDTO.investorTypeID == null || !await _validationService.CheckUUIDFormat(investorDTO.investorTypeID))
        //            throw new InvalidFieldException("Invalid investorTypeID!!!");

        //        if (!await _validationService.CheckExistenceId("InvestorType", Guid.Parse(investorDTO.investorTypeID)))
        //            throw new NotFoundException("This investorTypeID is not existed!!!");

        //        if (investorDTO.status < 0 || investorDTO.status > 2)
        //            throw new InvalidFieldException("Status must be 0(ACTIVE) or 1(INACTIVE) or 2(BLOCKED)!!!");

        //        if (investorDTO.createBy != null && investorDTO.createBy.Length >= 0)
        //        {
        //            if (investorDTO.createBy.Equals("string"))
        //                investorDTO.createBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investorDTO.createBy))
        //                throw new InvalidFieldException("Invalid createBy!!!");
        //        }

        //        if (investorDTO.updateBy != null && investorDTO.updateBy.Length >= 0)
        //        {
        //            if (investorDTO.updateBy.Equals("string"))
        //                investorDTO.updateBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investorDTO.updateBy))
        //                throw new InvalidFieldException("Invalid updateBy!!!");
        //        }

        //        Investor dto = _mapper.Map<Investor>(investorDTO);
        //        result = await _investorRepository.UpdateInvestor(dto, investorId);
        //        if (result == 0)
        //            throw new UpdateObjectException("Can not update Investor Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}
    }
}

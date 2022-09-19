using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
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
    public class InvestmentService : IInvestmentService
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IInvestorRepository _investorRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public InvestmentService(IInvestmentRepository investmentRepository, IInvestorRepository investorRepository, IPackageRepository packageRepository, IInvestorWalletRepository investorWalletRepository, IValidationService validationService, IMapper mapper)
        {
            _investorRepository = investorRepository;
            //_investorWalletRepository = investorWalletRepository;
            _investmentRepository = investmentRepository;
            _packageRepository = packageRepository;
            _investorWalletRepository = investorWalletRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CREATE
        public async Task<IdDTO> CreateInvestment(CreateInvestmentDTO investmentDTO, ThisUserObj currentUser)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (investmentDTO.investorId == null || !await _validationService.CheckUUIDFormat(investmentDTO.investorId))
                    throw new InvalidFieldException("Invalid investorId!!!");

                if (!await _validationService.CheckExistenceId("Investor", Guid.Parse(investmentDTO.investorId)))
                    throw new NotFoundException("This investorId is not existed!!!");

                if (investmentDTO.projectId == null || !await _validationService.CheckUUIDFormat(investmentDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(investmentDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (investmentDTO.packageId == null || !await _validationService.CheckUUIDFormat(investmentDTO.packageId))
                    throw new InvalidFieldException("Invalid packageId!!!");

                if (!await _validationService.CheckExistenceId("Package", Guid.Parse(investmentDTO.packageId)))
                    throw new NotFoundException("This packageId is not existed!!!");

                if (investmentDTO.quantity <= 0)
                    throw new InvalidFieldException("quantity must be greater than 0!!!");

                Package package = await _packageRepository.GetPackageById(Guid.Parse(investmentDTO.packageId));

                Investment entity = _mapper.Map<Investment>(investmentDTO);
                entity.TotalPrice = entity.Quantity * package.Price;
                entity.Status = InvestmentStatusEnum.WAITING.ToString();
                entity.CreateBy = Guid.Parse(currentUser.userId);
                entity.UpdateBy = Guid.Parse(currentUser.userId);

                newId.id = await _investmentRepository.CreateInvestment(entity);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Investment Object!");
                else
                {
                    InvestorWallet investorWallet = new InvestorWallet();
                    investorWallet.InvestorId = Guid.Parse(currentUser.investorId);
                    investorWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I3"));
                    investorWallet.Balance = entity.TotalPrice;
                    investorWallet.UpdateBy = Guid.Parse(currentUser.userId);

                    Investment investment = new Investment();
                    investment.Id = Guid.Parse(newId.id);
                    investment.Status = InvestmentStatusEnum.SUCCESS.ToString();
                    investment.UpdateBy = Guid.Parse(currentUser.userId);

                    if (await _investorWalletRepository.UpdateInvestorWalletBalance(investorWallet) == 1)
                    {
                        investment.Status = InvestmentStatusEnum.SUCCESS.ToString();
                        await _investmentRepository.UpdateInvestmentStatus(investment);
                    }
                    else
                    {
                        investment.Status = InvestmentStatusEnum.CANCELED.ToString();
                        await _investmentRepository.UpdateInvestmentStatus(investment);
                    }
                }

                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<GetInvestmentDTO>> GetAllInvestments(int pageIndex, int pageSize, ThisUserObj currentUser)
        {
            try
            {
                List<Investment> investmentList = await _investmentRepository.GetAllInvestments(pageIndex, pageSize);
                List<GetInvestmentDTO> list = _mapper.Map<List<GetInvestmentDTO>>(investmentList);

                foreach (GetInvestmentDTO item in list)
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
        public async Task<GetInvestmentDTO> GetInvestmentById(Guid investmentId, ThisUserObj currentUser)
        {
            GetInvestmentDTO result;
            try
            {
                Investment dto = await _investmentRepository.GetInvestmentById(investmentId);
                result = _mapper.Map<GetInvestmentDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No Investment Object Found!");

                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET FOR AUTHOR
        public async Task<List<InvestorInvestmentDTO>> GetInvestmentByProjectIdForAuthor(Guid projectId)
        {
            try
            {
                return await _investmentRepository.GetInvestmentByProjectIdForAuthor(projectId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET FOR WALLET
        public async Task<List<GetInvestmentDTO>> GetInvestmentForWallet(string walletType, ThisUserObj currentUser)
        {
            try
            {
                if (await _investorRepository.GetInvestorById(Guid.Parse(currentUser.investorId)) == null)
                {
                    throw new NotFoundException("No Investor Object Found!!!");
                }

                if (!walletType.Equals("I3") && !walletType.Equals("I4"))
                    throw new InvalidFieldException("walletType must be I3 or I4!!!");

                List<Investment> investmentList = await _investmentRepository.GetInvestmentForWallet(Guid.Parse(currentUser.investorId), walletType.Equals("I3") ? ProjectStatusEnum.CALLING_FOR_INVESTMENT.ToString() : ProjectStatusEnum.ACTIVE.ToString());
                List<GetInvestmentDTO> list = _mapper.Map<List<GetInvestmentDTO>>(investmentList);

                foreach (GetInvestmentDTO item in list)
                {
                    item.createDate = item.createDate == null ? null : await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = item.updateDate == null ? null : await _validationService.FormatDateOutput(item.updateDate);
                }

                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        //public async Task<int> UpdateInvestment(InvestmentDTO investmentDTO, Guid investmentId)
        //{
        //    int result;
        //    try
        //    {
        //        if (investmentDTO.investorId == null || !await _validationService.CheckUUIDFormat(investmentDTO.investorId))
        //            throw new InvalidFieldException("Invalid investorId!!!");

        //        if (!await _validationService.CheckExistenceId("Investor", Guid.Parse(investmentDTO.investorId)))
        //            throw new NotFoundException("This investorId is not existed!!!");

        //        if (investmentDTO.projectId == null || !await _validationService.CheckUUIDFormat(investmentDTO.projectId))
        //            throw new InvalidFieldException("Invalid projectId!!!");

        //        if (!await _validationService.CheckExistenceId("Project", Guid.Parse(investmentDTO.projectId)))
        //            throw new NotFoundException("This projectId is not existed!!!");

        //        if (investmentDTO.packageId == null || !await _validationService.CheckUUIDFormat(investmentDTO.packageId))
        //            throw new InvalidFieldException("Invalid packageId!!!");

        //        if (!await _validationService.CheckExistenceId("Package", Guid.Parse(investmentDTO.packageId)))
        //            throw new NotFoundException("This packageId is not existed!!!");

        //        if (investmentDTO.quantity <= 0)
        //            throw new InvalidFieldException("quantity must be greater than 0!!!");

        //        if (investmentDTO.createBy != null && investmentDTO.createBy.Length >= 0)
        //        {
        //            if (investmentDTO.createBy.Equals("string"))
        //                investmentDTO.createBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investmentDTO.createBy))
        //                throw new InvalidFieldException("Invalid createBy!!!");
        //        }

        //        if (investmentDTO.updateBy != null && investmentDTO.updateBy.Length >= 0)
        //        {
        //            if (investmentDTO.updateBy.Equals("string"))
        //                investmentDTO.updateBy = null;
        //            else if (!await _validationService.CheckUUIDFormat(investmentDTO.updateBy))
        //                throw new InvalidFieldException("Invalid updateBy!!!");
        //        }

        //        Investment dto = _mapper.Map<Investment>(investmentDTO);
        //        result = await _investmentRepository.UpdateInvestment(dto, investmentId);
        //        if (result == 0)
        //            throw new UpdateObjectException("Can not update Investment Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}
    }
}

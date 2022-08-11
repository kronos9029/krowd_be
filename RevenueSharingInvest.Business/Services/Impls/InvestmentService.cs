using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
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
        //private readonly IInvestorRepository _investorRepository;
        //private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public InvestmentService(IInvestmentRepository investmentRepository, IValidationService validationService, IMapper mapper)
        {
            //_investorRepository = investorRepository;
            //_investorWalletRepository = investorWalletRepository;
            _investmentRepository = investmentRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllInvestmentData()
        {
            int result;
            try
            {
                result = await _investmentRepository.ClearAllInvestmentData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateInvestment(InvestmentDTO investmentDTO)
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

                if (investmentDTO.createBy != null && investmentDTO.createBy.Length >= 0)
                {
                    if (investmentDTO.createBy.Equals("string"))
                        investmentDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(investmentDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (investmentDTO.updateBy != null && investmentDTO.updateBy.Length >= 0)
                {
                    if (investmentDTO.updateBy.Equals("string"))
                        investmentDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(investmentDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                investmentDTO.isDeleted = false;

                Investment dto = _mapper.Map<Investment>(investmentDTO);
                newId.id = await _investmentRepository.CreateInvestment(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Investment Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteInvestmentById(Guid investmentId)
        {
            int result;
            try
            {
                result = await _investmentRepository.DeleteInvestmentById(investmentId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete Investment Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<InvestmentDTO>> GetAllInvestments(int pageIndex, int pageSize)
        {
            try
            {
                List<Investment> investmentList = await _investmentRepository.GetAllInvestments(pageIndex, pageSize);
                List<InvestmentDTO> list = _mapper.Map<List<InvestmentDTO>>(investmentList);

                foreach (InvestmentDTO item in list)
                {
                    if (item.lastPayment != null)
                    {
                        item.lastPayment = await _validationService.FormatDateOutput(item.lastPayment);
                    }
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
        public async Task<InvestmentDTO> GetInvestmentById(Guid investmentId)
        {
            InvestmentDTO result;
            try
            {
                Investment dto = await _investmentRepository.GetInvestmentById(investmentId);
                result = _mapper.Map<InvestmentDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No Investment Object Found!");

                if (result.lastPayment != null)
                {
                    result.lastPayment = await _validationService.FormatDateOutput(result.lastPayment);
                }
                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

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

        //UPDATE
        public async Task<int> UpdateInvestment(InvestmentDTO investmentDTO, Guid investmentId)
        {
            int result;
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

                if (investmentDTO.createBy != null && investmentDTO.createBy.Length >= 0)
                {
                    if (investmentDTO.createBy.Equals("string"))
                        investmentDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(investmentDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (investmentDTO.updateBy != null && investmentDTO.updateBy.Length >= 0)
                {
                    if (investmentDTO.updateBy.Equals("string"))
                        investmentDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(investmentDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                Investment dto = _mapper.Map<Investment>(investmentDTO);
                result = await _investmentRepository.UpdateInvestment(dto, investmentId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Investment Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

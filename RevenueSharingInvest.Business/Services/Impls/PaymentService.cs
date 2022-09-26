using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
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
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public PaymentService(IPaymentRepository paymentRepository, IValidationService validationService, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllPaymentData()
        {
            int result;
            try
            {
                result = await _paymentRepository.ClearAllPaymentData();
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreatePayment(PaymentDTO paymentDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (paymentDTO.periodRevenueId == null || !await _validationService.CheckUUIDFormat(paymentDTO.periodRevenueId))
                    throw new InvalidFieldException("Invalid periodRevenueId!!!");

                if (!await _validationService.CheckExistenceId("PeriodRevenue", Guid.Parse(paymentDTO.periodRevenueId)))
                    throw new NotFoundException("This periodRevenueId is not existed!!!");

                if (paymentDTO.investmentId == null || !await _validationService.CheckUUIDFormat(paymentDTO.investmentId))
                    throw new InvalidFieldException("Invalid investmentId!!!");

                if (!await _validationService.CheckExistenceId("Investment", Guid.Parse(paymentDTO.investmentId)))
                    throw new NotFoundException("This investmentId is not existed!!!");

                if (paymentDTO.amount <= 0)
                    throw new InvalidFieldException("amount must be greater than 0!!!");

                if (paymentDTO.description != null && (paymentDTO.description.Equals("string") || paymentDTO.description.Length == 0))
                    paymentDTO.description = null;

                if (!await _validationService.CheckText(paymentDTO.type))
                    throw new InvalidFieldException("Invalid type!!!");

                if (paymentDTO.fromId == null || !await _validationService.CheckUUIDFormat(paymentDTO.fromId))
                    throw new InvalidFieldException("Invalid fromId!!!");

                if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(paymentDTO.fromId)))
                    throw new NotFoundException("This fromId is not existed!!!");

                if (paymentDTO.toId == null || !await _validationService.CheckUUIDFormat(paymentDTO.toId))
                    throw new InvalidFieldException("Invalid toId!!!");

                if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(paymentDTO.toId)))
                    throw new NotFoundException("This toId is not existed!!!");

                if (paymentDTO.createBy != null && paymentDTO.createBy.Length >= 0)
                {
                    if (paymentDTO.createBy.Equals("string"))
                        paymentDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(paymentDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (paymentDTO.updateBy != null && paymentDTO.updateBy.Length >= 0)
                {
                    if (paymentDTO.updateBy.Equals("string"))
                        paymentDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(paymentDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                paymentDTO.isDeleted = false;

                Payment dto = _mapper.Map<Payment>(paymentDTO);
                newId.id = await _paymentRepository.CreatePayment(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Payment Object!");
                return newId;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeletePaymentById(Guid paymentId)
        {
            int result;
            try
            {
                result = await _paymentRepository.DeletePaymentById(paymentId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete Payment Object!");
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PaymentDTO>> GetAllPayments(int pageIndex, int pageSize)
        {
            try
            {
                List<Payment> paymentList = await _paymentRepository.GetAllPayments(pageIndex, pageSize);
                List<PaymentDTO> list = _mapper.Map<List<PaymentDTO>>(paymentList);

                foreach (PaymentDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);
                }

                return list;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<PaymentDTO> GetPaymentById(Guid paymentId)
        {
            PaymentDTO result;
            try
            {
                Payment dto = await _paymentRepository.GetPaymentById(paymentId);
                result = _mapper.Map<PaymentDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No Payment Object Found!");

                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdatePayment(PaymentDTO paymentDTO, Guid paymentId)
        {
            int result;
            try
            {
                if (paymentDTO.periodRevenueId == null || !await _validationService.CheckUUIDFormat(paymentDTO.periodRevenueId))
                    throw new InvalidFieldException("Invalid periodRevenueId!!!");

                if (!await _validationService.CheckExistenceId("PeriodRevenue", Guid.Parse(paymentDTO.periodRevenueId)))
                    throw new NotFoundException("This periodRevenueId is not existed!!!");

                if (paymentDTO.investmentId == null || !await _validationService.CheckUUIDFormat(paymentDTO.investmentId))
                    throw new InvalidFieldException("Invalid investmentId!!!");

                if (!await _validationService.CheckExistenceId("Investment", Guid.Parse(paymentDTO.investmentId)))
                    throw new NotFoundException("This investmentId is not existed!!!");

                if (paymentDTO.amount <= 0)
                    throw new InvalidFieldException("amount must be greater than 0!!!");

                if (paymentDTO.description != null && (paymentDTO.description.Equals("string") || paymentDTO.description.Length == 0))
                    paymentDTO.description = null;

                if (!await _validationService.CheckText(paymentDTO.type))
                    throw new InvalidFieldException("Invalid type!!!");

                if (paymentDTO.fromId == null || !await _validationService.CheckUUIDFormat(paymentDTO.fromId))
                    throw new InvalidFieldException("Invalid fromId!!!");

                if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(paymentDTO.fromId)))
                    throw new NotFoundException("This fromId is not existed!!!");

                if (paymentDTO.toId == null || !await _validationService.CheckUUIDFormat(paymentDTO.toId))
                    throw new InvalidFieldException("Invalid toId!!!");

                if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(paymentDTO.toId)))
                    throw new NotFoundException("This toId is not existed!!!");

                if (paymentDTO.createBy != null && paymentDTO.createBy.Length >= 0)
                {
                    if (paymentDTO.createBy.Equals("string"))
                        paymentDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(paymentDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (paymentDTO.updateBy != null && paymentDTO.updateBy.Length >= 0)
                {
                    if (paymentDTO.updateBy.Equals("string"))
                        paymentDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(paymentDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                Payment dto = _mapper.Map<Payment>(paymentDTO);
                result = await _paymentRepository.UpdatePayment(dto, paymentId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Payment Object!");
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }
    }
}

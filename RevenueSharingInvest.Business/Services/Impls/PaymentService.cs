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
    }
}

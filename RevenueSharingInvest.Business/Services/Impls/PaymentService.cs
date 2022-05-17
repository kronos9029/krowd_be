using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
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
        private readonly IMapper _mapper;


        public PaymentService(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreatePayment(PaymentDTO paymentDTO)
        {
            int result;
            try
            {
                Payment dto = _mapper.Map<Payment>(paymentDTO);
                result = await _paymentRepository.CreatePayment(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Payment Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
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
                    throw new CreateObjectException("Can not delete Payment Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<PaymentDTO>> GetAllPayments()
        {
            List<Payment> paymentList = await _paymentRepository.GetAllPayments();
            List<PaymentDTO> list = _mapper.Map<List<PaymentDTO>>(paymentList);
            return list;
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
                    throw new CreateObjectException("No Payment Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdatePayment(PaymentDTO paymentDTO, Guid paymentId)
        {
            int result;
            try
            {
                Payment dto = _mapper.Map<Payment>(paymentDTO);
                result = await _paymentRepository.UpdatePayment(dto, paymentId);
                if (result == 0)
                    throw new CreateObjectException("Can not update Payment Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}

using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IPaymentService
    {
        //CREATE
        public Task<IdDTO> CreatePayment(PaymentDTO paymentDTO);

        //READ
        public Task<List<PaymentDTO>> GetAllPayments(int pageIndex, int pageSize);
        public Task<PaymentDTO> GetPaymentById(Guid paymentId);

        //UPDATE
        public Task<int> UpdatePayment(PaymentDTO paymentDTO, Guid paymentId);

        //DELETE
        public Task<int> DeletePaymentById(Guid paymentId);
        public Task<int> ClearAllPaymentData();
    }
}

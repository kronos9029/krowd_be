using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IPaymentRepository
    {
        //CREATE
        public Task<int> CreatePayment(Payment paymentDTO);

        //READ
        public Task<List<Payment>> GetAllPayments();
        public Task<Payment> GetPaymentById(Guid paymentId);

        //UPDATE
        public Task<int> UpdatePayment(Payment paymentDTO, Guid paymentId);

        //DELETE
        public Task<int> DeletePaymentById(Guid paymentId);
    }
}

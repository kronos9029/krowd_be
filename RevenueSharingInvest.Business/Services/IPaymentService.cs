using RevenueSharingInvest.API;
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

        //READ
        public Task<dynamic> GetAllPayments(int pageIndex, int pageSize, string type, ThisUserObj currentUser);
        public Task<PaymentDTO> GetPaymentById(Guid paymentId, ThisUserObj currentUser);

        //UPDATE

        //DELETE
    }
}

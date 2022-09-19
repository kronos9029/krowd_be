using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IMomoService
    {
        public Task<MomoPaymentResponse> RequestPaymentWeb(MomoPaymentRequest request);
        public Task<MomoPaymentResponse> RequestLinkAndPayment(MomoPaymentRequest request);
        public Task<QueryResponse> QueryTransactionStatus(QueryRequest request);
        public Task<ConfirmResponse> ConfirmMomoTransaction(ConfirmRequest request);
    }
}

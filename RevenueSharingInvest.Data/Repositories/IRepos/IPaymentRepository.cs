﻿using RevenueSharingInvest.Data.Models.Entities;
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
        public Task<string> CreatePayment(Payment paymentDTO);

        //READ
        public Task<List<Payment>> GetAllPayments(int pageIndex, int pageSize, string type, Guid roleId, Guid userId);
        public Task<Payment> GetPaymentById(Guid paymentId);

        //UPDATE

        //DELETE
    }
}

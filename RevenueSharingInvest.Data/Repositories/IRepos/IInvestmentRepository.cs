﻿using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IInvestmentRepository
    {
        //CREATE
        public Task<int> CreateInvestment(Investment investmentDTO);

        //READ
        public Task<List<Investment>> GetAllInvestments();
        public Task<Investment> GetInvestmentById(Guid investmentId);

        //UPDATE
        public Task<int> UpdateInvestment(Investment investmentDTO, Guid investmentId);

        //DELETE
        public Task<int> DeleteInvestmentById(Guid investmentId);
    }
}
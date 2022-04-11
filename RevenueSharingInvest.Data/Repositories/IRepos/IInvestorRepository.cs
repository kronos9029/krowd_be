﻿using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IInvestorRepository
    {
        Task<int> CreateInvestorType(InvestorType investorType);
        Task<int> CreateInvestor(Investor investor);
        Task<List<InvestorType>> GetAllInvestorType();

    }
}

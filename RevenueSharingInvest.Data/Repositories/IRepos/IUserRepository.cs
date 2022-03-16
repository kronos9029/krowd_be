﻿using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IUserRepository
    {
        public Task<User> GetUserByEmail(String userEmail);
        public Task<int> CreateInvestorUser(User newUser);
        public Task<int> CreateBusinessUser(User newUser);
    }
}

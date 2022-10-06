using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IInvestorRepository
    {
        //CREATE
        public Task<string> CreateInvestor(Investor investorDTO);

        //READ
        public Task<Investor> GetInvestorById(Guid investorId);
        public Task<Guid> GetInvestorByEmail(String email);
        public Task<Investor> GetInvestorByUserId(Guid userId);

        //UPDATE
        public Task<int> UpdateInvestorStatus(Guid userId, string status, Guid currentUserId);

        //DELETE
    }
}

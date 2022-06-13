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
        public Task<List<Investor>> GetAllInvestors(int pageIndex, int pageSize);
        public Task<Investor> GetInvestorById(Guid investorId);

        //UPDATE
        public Task<int> UpdateInvestor(Investor investorDTO, Guid investorId);

        //DELETE
        public Task<int> DeleteInvestorById(Guid investorId);
        public Task<int> ClearAllInvestorData();
    }
}

using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IInvestorTypeRepository
    {
        //CREATE
        public Task<int> CreateInvestorType(InvestorType investorTypeDTO);

        //READ
        public Task<List<InvestorType>> GetAllInvestorTypes();
        public Task<InvestorType> GetInvestorTypeById(Guid investorTypeId);

        //UPDATE
        public Task<int> UpdateInvestorType(InvestorType investorTypeDTO, Guid investorTypeId);

        //DELETE
        public Task<int> DeleteInvestorTypeById(Guid investorTypeId);
    }
}

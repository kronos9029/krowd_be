using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IInvestorTypeService
    {
        //CREATE
        public Task<int> CreateInvestorType(InvestorTypeDTO investorTypeDTO);

        //READ
        public Task<List<InvestorTypeDTO>> GetAllInvestorTypes();
        public Task<InvestorTypeDTO> GetInvestorTypeById(Guid investorTypeId);

        //UPDATE
        public Task<int> UpdateInvestorType(InvestorTypeDTO investorTypeDTO, Guid investorTypeId);

        //DELETE
        public Task<int> DeleteInvestorTypeById(Guid investorTypeId);
    }
}

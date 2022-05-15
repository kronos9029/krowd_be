using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IInvestorService
    {
        //public Task<InvestorDTO> GetInvestorById(String ID);
        //public Task<List<ProjectMemberDTO>> GetProjectMembers(String projectID);
        //CREATE
        public Task<int> CreateInvestor(InvestorDTO investorDTO);

        //READ
        public Task<List<InvestorDTO>> GetAllInvestors();
        public Task<InvestorDTO> GetInvestorById(Guid investorId);

        //UPDATE
        public Task<int> UpdateInvestor(InvestorDTO investorDTO, Guid investorId);

        //DELETE
        public Task<int> DeleteInvestorById(Guid investorId);
    }
}

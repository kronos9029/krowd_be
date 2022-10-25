using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
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
        public Task<string> CreateInvestment(Investment investmentDTO);

        //READ
        public Task<List<Investment>> GetAllInvestments(int pageIndex, int pageSize, string walletTypeId, string businessId, string projectId, string investorId, Guid roleId);
        public Task<Investment> GetInvestmentById(Guid investmentId);
        public Task<List<InvestorInvestmentDTO>> GetInvestmentByProjectIdForAuthor(Guid projectId);
        public Task<List<InvestedRecord>> GetInvestmentRecord(Guid projectId, Guid investorId);
        //public Task<List<Investment>> GetInvestmentForWallet(Guid investorId, string status);

        //UPDATE
        //public Task<int> UpdateInvestment(Investment investmentDTO, Guid investmentId);
        public Task<int> UpdateInvestmentStatus(Investment investmentDTO);

        //DELETE
        //public Task<int> DeleteInvestmentById(Guid investmentId);
        //public Task<int> ClearAllInvestmentData();
    }
}

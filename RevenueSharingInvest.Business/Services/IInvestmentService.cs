using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IInvestmentService
    {
        //CREATE
        public Task<IdDTO> CreateInvestment(CreateInvestmentDTO investmentDTO, ThisUserObj currentUser);

        //READ
        public Task<List<GetInvestmentDTO>> GetAllInvestments(int pageIndex, int pageSize, string walletTypeId, string businessId, string projectId, string investorId, ThisUserObj currentUser);
        public Task<GetInvestmentDTO> GetInvestmentById(Guid investmentId, ThisUserObj currentUser);
        //public Task<List<InvestorDTO>> GetProjectMember(String projectID);
        public Task<List<InvestorInvestmentDTO>> GetInvestmentByProjectIdForAuthor(Guid projectId);
        //public Task<List<GetInvestmentDTO>> GetInvestmentForWallet(string walletType, ThisUserObj currentUser);

        //UPDATE
        //public Task<int> UpdateInvestment(InvestmentDTO investmentDTO, Guid investmentId);

        //DELETE
        //public Task<int> DeleteInvestmentById(Guid investmentId);
        //public Task<int> ClearAllInvestmentData();
    }
}

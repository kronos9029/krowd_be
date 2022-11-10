using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
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
        public Task<InvestmentPaymentDTO> CreateInvestment(CreateInvestmentDTO investmentDTO, ThisUserObj currentUser);

        //READ
        public Task<AllInvestmentDTO> GetAllInvestments(int pageIndex, int pageSize, string walletTypeId, string businessId, string projectId, string investorId, string status, ThisUserObj currentUser);
        public Task<GetInvestmentDTO> GetInvestmentById(Guid investmentId, ThisUserObj currentUser);
        //public Task<List<InvestorDTO>> GetProjectMember(String projectID);
        public Task<List<InvestorInvestmentDTO>> GetInvestmentByProjectIdForAuthor(Guid projectId);
        //public Task<List<GetInvestmentDTO>> GetInvestmentForWallet(string walletType, ThisUserObj currentUser);

        //UPDATE
        public Task<int> CancelInvestment(Guid investmentId, ThisUserObj currentUser);

        //DELETE
        //public Task<int> DeleteInvestmentById(Guid investmentId);
        //public Task<int> ClearAllInvestmentData();
    }
}

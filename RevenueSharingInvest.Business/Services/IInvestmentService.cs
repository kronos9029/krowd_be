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
        public Task<int> CreateInvestment(InvestmentDTO investmentDTO);

        //READ
        public Task<List<InvestmentDTO>> GetAllInvestments();
        public Task<InvestmentDTO> GetInvestmentById(Guid investmentId);
        //public Task<List<InvestorDTO>> GetProjectMember(String projectID);

        //UPDATE
        public Task<int> UpdateInvestment(InvestmentDTO investmentDTO, Guid investmentId);

        //DELETE
        public Task<int> DeleteInvestmentById(Guid investmentId);
    }
}

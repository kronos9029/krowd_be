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
        public Task<InvestorDTO> GetInvestorById(String ID);
        public Task<List<ProjectMemberDTO>> GetProjectMembers(String projectID);
    }
}

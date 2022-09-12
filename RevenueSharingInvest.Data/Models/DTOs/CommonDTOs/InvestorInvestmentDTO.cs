using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class InvestorInvestmentDTO
    {
        public Guid InvestorId { get; set; }
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
    }
}

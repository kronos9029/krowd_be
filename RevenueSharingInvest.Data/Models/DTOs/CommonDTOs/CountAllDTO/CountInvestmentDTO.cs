using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class CountInvestmentDTO
    {
        public int waiting { get; set; }
        public int success { get; set; }
        public int failed { get; set; }
        public int canceled { get; set; }
    }
}

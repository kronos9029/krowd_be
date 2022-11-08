using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class AllInvestmentDTO
    {
        public int numOfInvestment { get; set; }
        public List<GetInvestmentDTO> listOfInvestment { get; set; }
    }
}

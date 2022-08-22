using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class AllInvestorDTO
    {
        public int numOfInvestor { get; set; }
        public List<GetInvestorDTO> listOfInvestor { get; set; }
    }
}

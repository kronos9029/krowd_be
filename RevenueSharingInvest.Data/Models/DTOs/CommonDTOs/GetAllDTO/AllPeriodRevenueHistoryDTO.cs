using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs.GetAllDTO
{
    public class AllPeriodRevenueHistoryDTO
    {
        public int numOfPeriodRevenueHistory { get; set; }
        public List<PeriodRevenueHistoryDTO> listOfPeriodRevenueHistory { get; set; }
    }
}

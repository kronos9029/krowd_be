using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class NewProjectDTO
    {
        public ProjectDTO project { get; set; }
        public List<StageDTO> stageList { get; set; }
        public List<PackageDTO> packageList { get; set; }
        //public List<PeriodRevenueDTO> periodRevenueList { get; set; }
        //public ProjectEntityDTO projectEntity { get; set; }
    }
}

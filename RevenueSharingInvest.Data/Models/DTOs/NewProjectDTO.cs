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
        public StageDTO stage { get; set; }
        public List<PackageDTO> packageList { get; set; }
        public PeriodRevenueDTO periodRevenue { get; set; }
        public List<ProjectEntityDTO> projectEntityList { get; set; }
    }
}

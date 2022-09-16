using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class StageChartDTO
    {
        public string chartName { get; set; }
        public List<StageLineDTO> lineList { get; set; }
    }
}

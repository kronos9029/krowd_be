using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class AllStageDTO
    {
        public int numOfStage { get; set; }
        public List<GetStageDTO> listOfStage { get; set; }
        public CountStageDTO filterCount { get; set; }
    }
}

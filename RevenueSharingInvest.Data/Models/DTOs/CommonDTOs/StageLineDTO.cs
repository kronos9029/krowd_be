using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class StageLineDTO
    {
        public string lineName { get; set; }
        public List<float> data { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class CountStageDTO
    {
        public int inactive { get; set; }
        public int undue { get; set; }
        public int due { get; set; }
        public int done { get; set; }
    }
}

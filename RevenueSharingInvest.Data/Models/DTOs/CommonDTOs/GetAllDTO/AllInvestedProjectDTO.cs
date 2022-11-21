using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class AllInvestedProjectDTO
    {
        public int numOfProject { get; set; }
        public List<InvestedProjectDTO> listOfProject { get; set; }
    }
}

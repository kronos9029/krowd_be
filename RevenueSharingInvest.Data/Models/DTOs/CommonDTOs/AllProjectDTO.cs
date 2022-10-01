using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class AllProjectDTO
    {
        public int numOfProject { get; set; }
        public List<BasicProjectDTO> listOfProject { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class AllBusinessDTO
    {
        public int numOfBusiness { get; set; }
        public List<GetBusinessDTO> listOfBusiness { get; set; }
    }
}

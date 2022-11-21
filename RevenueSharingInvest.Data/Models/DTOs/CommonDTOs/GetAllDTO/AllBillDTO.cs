using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class AllBillDTO
    {
        public int numOfBill { get; set; }
        public List<GetBillDTO> listOfBill { get; set; }
    }
}

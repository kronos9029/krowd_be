using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class AllFieldDTO
    {
        public int numOfField { get; set; }
        public List<FieldDTO> listOfField{ get; set; }
    }
}

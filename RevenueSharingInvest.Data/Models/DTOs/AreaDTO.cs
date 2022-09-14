using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class AreaDTO
    {
        
        public string city { get; set; }
        public string district { get; set; }
        
    }

    public class CreateUpdateAreaDTO : AreaDTO
    {

    }

    public class GetAreaDTO : AreaDTO
    {
        public string id { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }
}

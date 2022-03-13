using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class WalletTypeDTO
    {
        public string id { get; set; }
        public string name { get; set; }//ví đầu tư chung, ví tạm, ví thanh toán chung
        public string description { get; set; }
        public string mode { get; set; }//project, investor, krowd
        public string type { get; set; }//I1 - I5, B1 - B4
    }
}

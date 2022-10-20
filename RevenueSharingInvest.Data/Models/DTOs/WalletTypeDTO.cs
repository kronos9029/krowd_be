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
        public string name { get; set; }
        public string description { get; set; }
        public string mode { get; set; }//BUSINESS, INVESTOR, SYSTEM
        public string type { get; set; }     
    }

    public class GetWalletTypeDTO : WalletTypeDTO
    {
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }

    public class GetWalletTypeForWalletDTO : WalletTypeDTO
    {

    }
}

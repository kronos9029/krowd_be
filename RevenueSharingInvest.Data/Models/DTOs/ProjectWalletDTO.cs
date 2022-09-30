using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class ProjectWalletDTO
    {
        public string id { get; set; }
        public string projectManagerId { get; set; }
        public double balance { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }

    public class MappedProjectWalletDTO : ProjectWalletDTO
    {
        public string walletTypeId { get; set; }
    }

    public class GetProjectWalletDTO : ProjectWalletDTO
    {
        public GetWalletTypeForWalletDTO walletType { get; set; }
    }
}

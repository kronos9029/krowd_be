using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class UserWalletsDTO
    {
        public float totalAsset { get; set; }
        public List<GetInvestorWalletDTO> listOfInvestorWallet { get; set; }
        public List<GetProjectWalletDTO> listOfProjectWallet { get; set; }
    }
}

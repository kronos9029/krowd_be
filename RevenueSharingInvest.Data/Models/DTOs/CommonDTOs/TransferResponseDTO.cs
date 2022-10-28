using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class TransferResponseDTO
    {
        public Guid fromWalletId { get; set; }
        public string fromWalletName { get; set; }
        public string fromWalletType { get; set; }
        public Guid toWalletId { get; set; }
        public string toWalletName { get; set; }
        public string toWalletType { get; set; }
        public double amount { get; set; }

    }
}

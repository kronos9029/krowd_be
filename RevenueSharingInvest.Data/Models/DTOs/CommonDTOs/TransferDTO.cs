using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class TransferDTO
    {
        public Guid fromWalletId { get; set; }
        public Guid toWalletId { get; set; }
        public double amount { get; set; }
    }
}

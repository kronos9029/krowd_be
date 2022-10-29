using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class AllWalletTransactionDTO
    {
        public int numOfWalletTransaction { get; set; }
        public List<WalletTransactionDTO> listOfWalletTransaction { get; set; }
    }
}

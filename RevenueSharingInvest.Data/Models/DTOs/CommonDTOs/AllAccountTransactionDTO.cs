using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class AllAccountTransactionDTO
    {
        public int numOfAccountTransaction { get; set; }
        public List<AccountTransactionDTO> listOfAccountTransaction { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class CountWalletTransactionDTO
    {
        public int all { get; set; }
        public int? i1 { get; set; }
        public int? i2 { get; set; }
        public int? i3 { get; set; }
        public int? i4 { get; set; }
        public int? i5 { get; set; }
        public int? p1 { get; set; }
        public int? p2 { get; set; }
        public int? p3 { get; set; }
        public int? p4 { get; set; }
        public int? p5 { get; set; }
        public int cashIn { get; set; }
        public int cashOut { get; set; }
        public int deposit { get; set; }
        public int withdraw { get; set; }
    }
}

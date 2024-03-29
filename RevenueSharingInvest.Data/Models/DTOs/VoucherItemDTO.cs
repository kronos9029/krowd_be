﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class VoucherItemDTO
    {
        public string id { get; set; }
        public string voucherId { get; set; }
        public string investmentId { get; set; }
        public string issuedDate { get; set; }
        public string expireDate { get; set; }
        public string redeemDate { get; set; }
        public string availableDate { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }
}

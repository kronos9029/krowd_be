﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class PaymentDTO
    {
        public string id { get; set; }
        public string investmentId { get; set; }
        public string periodRevenueId { get; set; }
        public float amount { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public float fromId { get; set; }
        public float toId { get; set; }
        public DateTime createDate { get; set; }
        public string createBy { get; set; }
        public DateTime updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
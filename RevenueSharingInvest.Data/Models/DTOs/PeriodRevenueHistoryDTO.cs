﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class PeriodRevenueHistoryDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string periodRevenueId { get; set; }
        public double amount { get; set; }
        public string description { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
    }

    public class CreatePeriodRevenueHistoryDTO
    {
        public string stageId { get; set; }
        public double amount { get; set; }
    }
}

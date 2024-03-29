﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class PackageDTO
    {
        public string name { get; set; }        
        public double price { get; set; }
        public string image { get; set; }
        public int quantity { get; set; }        
    }

    public class CreatePackageDTO : PackageDTO
    {
        public string projectId { get; set; }
        public string description { get; set; }
    }

    public class UpdatePackageDTO : PackageDTO
    {
        public string description { get; set; }
    }

    public class GetPackageDTO : PackageDTO
    {
        public string id { get; set; }
        public List<string> descriptionList { get; set; }
        public int remainingQuantity { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }

    public class PackagePercentDTO
    {
        public Guid id { get; set; }
        public int numOfPackage { get; set; }
        public float percent { get; set; }
        public double paidAmount { get; set; }
        public double paidPerInvestment { get; set; }
    }
}

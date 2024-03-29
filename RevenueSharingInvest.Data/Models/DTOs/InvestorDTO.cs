﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class InvestorDTO
    {
        public string userID { get; set; }
    }

    public class CreateInvestorDTO : InvestorDTO
    {
        public string investorTypeID { get; set; }
    }

    public class GetInvestorDTO : InvestorDTO
    {
        public string id { get; set; }
        public UserInvestorTypeDTO investorType { get; set; }
        public int status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }
}

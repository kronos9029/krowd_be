﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class AllWithdrawRequestDTO
    {
        public int numOfWithdrawRequest { get; set; }
        public List<GetWithdrawRequestDTO> listOfWithdrawRequest { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class AllUserDTO
    {
        public int numOfUser { get; set; }
        public List<GetUserDTO> listOfUser { get; set; }
    }
}

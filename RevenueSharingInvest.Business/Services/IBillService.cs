﻿using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IBillService
    {
        public Task<DailyReportDTO> BulkInsertBills(InsertBillDTO bills, string projectId, string date);
    }
}

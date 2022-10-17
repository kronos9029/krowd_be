﻿using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Bill
    {
        public Guid Id { get; set; }
        public string InvoiceId { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}

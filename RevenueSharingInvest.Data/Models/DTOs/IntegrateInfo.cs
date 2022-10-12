using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class IntegrateInfo
    {
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get;set; }
    }
}

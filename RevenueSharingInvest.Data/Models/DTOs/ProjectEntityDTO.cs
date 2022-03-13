using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class ProjectEntityDTO
    {
        public string id { get; set; }
        public string projectId { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public DateTime createDate { get; set; }
        public string createBy { get; set; }
        public DateTime updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}

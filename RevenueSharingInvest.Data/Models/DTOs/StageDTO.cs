using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class StageDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string projectId { get; set; }
        public string description { get; set; }
        public float percents { get; set; }
        public int openMonth { get; set; }
        public int closeMonth { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}

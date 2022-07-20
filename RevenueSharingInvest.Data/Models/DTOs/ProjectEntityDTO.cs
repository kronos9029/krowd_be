using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class ProjectEntityDTO : FirebasePath
    {
        public string id { get; set; }
        public string projectId { get; set; }
        public string title { get; set; }
        public IFormFile? image { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }

}

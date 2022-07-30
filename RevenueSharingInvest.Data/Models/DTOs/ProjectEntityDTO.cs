using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class ProjectEntityDTO
    {  
        public string title { get; set; }
        public string link { get; set; }
        public string content { get; set; }
        public string description { get; set; }             
    }

    public class ProjectComponentProjectEntityDTO : ProjectEntityDTO
    {
        public string id { get; set; }
        public int priority { get; set; }
    }
    public class CreateUpdateProjectEntityDTO : ProjectEntityDTO
    {
        public string projectId { get; set; }
        public string type { get; set; }
    }

    public class GetProjectEntityDTO : ProjectEntityDTO
    {
        public string id { get; set; }
        public int priority { get; set; }
        public string projectId { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }

    public class ProjectEntityFile
    {
        public string id { get; set; }
        public string projectId { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public List<IFormFile> files { get; set; }
    }

}

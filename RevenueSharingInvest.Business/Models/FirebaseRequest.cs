using Microsoft.AspNetCore.Http;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Models
{
    public class FirebaseRequest : CreateProjectEntityDTO
    {
        public string businessId { get; set; }
        public string createBy { get; set; }
        public string createDate { get; set; }
        public string updateBy { get; set; }
        public string updateDate { get; set; }
        public string entityName { get; set; }
        public string entityId { get; set; }
        public List<IFormFile> files { get; set; }

    }
}

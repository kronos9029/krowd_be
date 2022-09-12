using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class AuthenticateResponse
    {
        public Guid id { get; set; }
        public Guid? investorId { get; set; }
        public Guid? businessId { get; set; }
        public Guid? roleId { get; set; }
        public string roleName { get; set; }
        public string uid { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string phoneNum { get; set; }
        public string image { get; set; }
        public string provider { get; set; }
        public string token { get; set; }
    }
}

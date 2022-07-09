using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class UserDTO
    {
        public string id { get; set; }
        public string businessId { get; set; }
        public string roleId { get; set; }
        public string description { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string phoneNum { get; set; }
        public IFormFile image { get; set; }
        public string idCard { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string dateOfBirth { get; set; }
        public string taxIdentificationNumber { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public string bankName { get; set; }
        public string bankAccount { get; set; }
        public int status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}

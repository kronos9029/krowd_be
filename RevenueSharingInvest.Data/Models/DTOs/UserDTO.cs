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
        public string lastName { get; set; }
        public string firstName { get; set; }
    }

    public class CreateUserDTO : UserDTO
    {
        public string businessId { get; set; }
        public string email { get; set; }
        public string image { get; set; }
    }

    public class UpdateUserDTO : UserDTO
    {
        public IFormFile image { get; set; }
        public string businessId { get; set; }
        public string description { get; set; }
        public string phoneNum { get; set; }
        public string idCard { get; set; }
        public string gender { get; set; }
        public string dateOfBirth { get; set; }
        public string taxIdentificationNumber { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public string bankName { get; set; }
        public string bankAccount { get; set; }
    }

    public class GetUserDTO : UserDTO
    {
        public string id { get; set; }
        public GetBusinessDTO? business { get; set; }
        public RoleDTO role { get; set; }
        //public GetInvestorDTO? investor { get; set; }
        public string description { get; set; }
        public string phoneNum { get; set; }
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
        public string image { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }

    public class ProjectMemberUserDTO : UserDTO
    {
        public string image { get; set; }
        public string investDate { get; set; }
    }

    public class BusinessManagerUserDTO : UserDTO
    {
        public string id { get; set; }
        public string description { get; set; }
        public string phoneNum { get; set; }
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
        public string image { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }

    public class ProjectManagerUserDTO : UserDTO
    {
        public string id { get; set; }
        public string description { get; set; }
        public string phoneNum { get; set; }
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
        public string image { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}

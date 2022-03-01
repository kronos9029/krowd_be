using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class InvestorDTO
    {
        public string ID { get; set; }
        public string userID { get; set; }
        public string investorTypeID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string phoneNum { get; set; }
        public string image { get; set; }
        public string IDCard { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string dateOfBirth { get; set; }
        public string taxIdentificationNumber { get; set; }
        public string address { get; set; }
        public string bank { get; set; }
        public string bankAccount { get; set; }
        public DateTime createDate { get; set; }
        public string createBy { get; set; }
        public DateTime updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
        public float balance { get; set; }


    }
}

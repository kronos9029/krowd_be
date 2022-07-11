﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class BusinessDetailDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public UserDTO manager { get; set; }
        public List<FieldDTO> fieldList { get; set; }
        public string phoneNum { get; set; }
        public string image { get; set; }
        public string email { get; set; }
        public string description { get; set; }
        public string taxIdentificationNumber { get; set; }
        public string address { get; set; }
        public int numOfProject { get; set; }
        public int numOfSuccessfulProject { get; set; }
        public float successfulRate { get; set; }
        public int status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
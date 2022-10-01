using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class ProjectDTO
    {        
        public string name { get; set; }
        public string description { get; set; }
        public string address { get; set; }       
        public double investmentTargetCapital { get; set; }
        public float sharedRevenue { get; set; }
        public float multiplier { get; set; }
        public int duration { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
    }

    public class CreateProjectDTO : ProjectDTO
    {
        public int numOfStage { get; set; }
        public string managerId { get; set; }
        public string fieldId { get; set; }
        public string areaId { get; set; }
    }

    public class UpdateProjectDTO : ProjectDTO
    {
        public IFormFile image { get; set; }
        //public string managerId { get; set; }
        //public string fieldId { get; set; }
        //public string areaId { get; set; }
        public int numOfStage { get; set; }
        public string businessLicense { get; set; }
    }

    public class GetProjectDTO : ProjectDTO
    {
        public string id { get; set; }
        public string image { get; set; }
        public GetBusinessDTO business { get; set; }
        public ProjectManagerUserDTO manager { get; set; }
        public FieldDTO field { get; set; }
        public AreaDTO area { get; set; }
        public List<TypeProjectEntityDTO> projectEntity { get; set; }
        public List<ProjectMemberUserDTO> memberList { get; set; }
        public double investedCapital { get; set; }
        public int numOfStage { get; set; }
        public double remainAmount { get; set; }
        public string businessLicense { get; set; }
        public string approvedDate { get; set; }
        public string approvedBy { get; set; }
        public string status { get; set; }
        public List<string> tagList { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }

    public class InvestedProjectDTO : ProjectDTO
    {
        public string id { get; set; }
        public string managerId { get; set; }
        public string fieldId { get; set; }
        public string areaId { get; set; }
        public string image { get; set; }
        public double investedCapital { get; set; }
        public int numOfStage { get; set; }
        public double remainAmount { get; set; }
        public string businessLicense { get; set; }
        public string approvedDate { get; set; }
        public string approvedBy { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
        public double investedAmount { get; set; }
        public double receivedAmount { get; set; }
        public string lastedInvestedDate { get; set; }
    }

    public class BasicProjectDTO : ProjectDTO
    {
        public string id { get; set; }
        public string businessId { get; set; }
        public string businessName { get; set; }
        public string businessImage { get; set; }
        public string managerId { get; set; }
        public string fieldId { get; set; }
        public string fieldName { get; set; }
        public string fieldDescription { get; set; }
        public string areaId { get; set; }
        public string image { get; set; }
        public double investedCapital { get; set; }
        public int numOfStage { get; set; }
        public double remainAmount { get; set; }
        public string approvedDate { get; set; }
        public string approvedBy { get; set; }
        public string status { get; set; }
        public List<string> tagList { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}

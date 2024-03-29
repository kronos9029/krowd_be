﻿using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IProjectRepository
    {
        //CREATE
        public Task<string> CreateProject(Project projectDTO);

        //READ
        public Task<List<Project>> GetAllProjects
        (
            int pageIndex, 
            int pageSize, 
            string businessId, 
            string managerId, 
            string areaId, 
            List<string> listFieldId,
            double investmentTargetCapital,
            string name, 
            string status, 
            string roleId
        );
        public Task<List<Project>> GetOutstadingProjects();
        public Task<Project> GetProjectById(Guid projectId);
        public Task<int> CountProject
        (
            string businessId,
            string managerId,
            string areaId,
            List<string> listFieldId,
            double investmentTargetCapital,
            string name,
            string status,
            string roleId
        );
        public Task<List<Project>> GetInvestedProjects(int pageIndex, int pageSize, Guid investorId);
        public Task<int> CountInvestedProjects(Guid investorId);
        public Task<List<BusinessProjectDTO>> GetBusinessProjectsToAuthor(Guid businessId);
        public Task<IntegrateInfo> GetIntegrateInfoByProjectId(Guid projectId);
        public Task<string> GetProjectNameForContractById(Guid projectId);
        public Task<Project> GetProjectByDailyReportId(Guid dailyReportId);
        public Task<InvestedProjectDetail> GetInvestedProjectDetail(Guid projectId, Guid investorId);
        public Task<double> GetReturnedDeptOfOneInvestor(Guid projectId, Guid userId);
        public Task<double> GetReturnedDeptOfOneInvestorByStage(Guid stageId, Guid userId);
        public Task<string> GetPrjectImageByProjectId(Guid projectId);
        public Task<NumOfPaidStageAndLastPaymentDate> GetNumOfPaidStageAndLastPaymentDate(Guid projectId);

        //UPDATE
        public Task<int> UpdateProject(Project projectDTO, Guid projectId);
        public Task<int> UpdateProjectImage(string url, Guid projectId);
        public Task<int> UpdateProjectStatus(Guid projectId, string status, Guid updaterId);
        public Task<int> UpdateProjectInvestedCapital(Guid projectId, double investedAmount, Guid updateBy);
        public Task<int> UpdateProjectRemainingAmount(Project projectDTO);
        public Task<int> ApproveProject(Guid projectId, Guid updateBy);

        //DELETE
        public Task<int> DeleteProjectById(Guid projectId);
    }
}

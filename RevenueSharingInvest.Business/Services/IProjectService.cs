using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IProjectService
    {
        //CREATE
        public Task<IdDTO> CreateProject(CreateProjectDTO projectDTO, ThisUserObj thisUserObj);

        //READ
        public Task<AllProjectDTO> GetAllProjects
        (
            int pageIndex, 
            int pageSize, 
            string businessId, 
            string areaId, 
            string fieldId, 
            string name,
            string status,
            ThisUserObj thisUserObj
        );
        public Task<AllInvestedProjectDTO> GetInvestedProjects(int pageIndex, int pageSize, ThisUserObj thisUserObj);
        public Task<GetProjectDTO> GetProjectById(Guid projectId);
        public Task<ProjectCountDTO> CountProjects
        (
            string businessId, 
            string areaId, 
            string fieldId, 
            string name,
            string status,
            ThisUserObj thisUserObj
        );
        public Task<List<BusinessProjectDTO>> GetBusinessProjectsToAuthor(Guid businessId);
        public Task<IntegrateInfo> GetIntegrateInfoByUserEmail(string projectId);

        //UPDATE
        public Task<int> UpdateProject(UpdateProjectDTO projectDTO, Guid projectId, ThisUserObj thisUserObj);
        public Task<int> UpdateProjectStatus(Guid projectId, string status, ThisUserObj currentUser);

        //DELETE
        public Task<int> DeleteProjectById(Guid projectId);
    }
}

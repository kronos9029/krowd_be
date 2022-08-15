using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
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
        public Task<IdDTO> CreateProject(CreateUpdateProjectDTO projectDTO, ThisUserObj thisUserObj);

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

        //UPDATE
        public Task<int> UpdateProject(CreateUpdateProjectDTO projectDTO, Guid projectId);

        //DELETE
        public Task<int> DeleteProjectById(Guid projectId);
        public Task<int> ClearAllProjectData();
    }
}

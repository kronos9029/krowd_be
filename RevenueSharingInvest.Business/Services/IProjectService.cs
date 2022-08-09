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
        public Task<IdDTO> CreateProject(CreateUpdateProjectDTO projectDTO);

        //READ
        public Task<AllProjectDTO> GetAllProjects(int pageIndex, int pageSize, string businessId, string managerId, string areaId, string fieldId, string investorId,string temp_field_role);
        public Task<GetProjectDTO> GetProjectById(Guid projectId);
        public Task<int> CountProjects(string businessId, string managerId, string areaId, string fieldId, string investorId, string temp_field_role);

        //UPDATE
        public Task<int> UpdateProject(CreateUpdateProjectDTO projectDTO, Guid projectId);

        //DELETE
        public Task<int> DeleteProjectById(Guid projectId);
        public Task<int> ClearAllProjectData();
    }
}

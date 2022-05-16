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
        public Task<int> CreateProject(ProjectDTO projectDTO);

        //READ
        public Task<List<ProjectDTO>> GetAllProjects();
        public Task<ProjectDTO> GetProjectById(Guid projectId);

        //UPDATE
        public Task<int> UpdateProject(ProjectDTO projectDTO, Guid projectId);

        //DELETE
        public Task<int> DeleteProjectById(Guid projectId);
    }
}

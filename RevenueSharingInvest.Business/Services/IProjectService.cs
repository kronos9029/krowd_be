using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;

namespace RevenueSharingInvest.Business.Services
{
    public interface IProjectService
    {
        //CREATE
        public Task<int> CreateNewProject(NewProjectDTO newProjectDTO);
        public Task<int> CreateProject(ProjectDTO projectDTO);

        //READ
        public Task<List<ProjectDTO>> GetAllProjects();

        //UPDATE
        public Task<int> UpdateProject(ProjectDTO projectDTO, Guid projectId);

        //DELETE
        public Task<int> DeleteProjectById(Guid projectId);
    }
}

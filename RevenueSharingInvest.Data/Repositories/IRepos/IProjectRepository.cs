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
        public Task<List<Project>> GetAllProjects(int pageIndex, int pageSize);
        public Task<Project> GetProjectById(Guid projectId);

        //UPDATE
        public Task<int> UpdateProject(Project projectDTO, Guid projectId);

        //DELETE
        public Task<int> DeleteProjectById(Guid projectId);
        public Task<int> ClearAllProjectData();
    }
}

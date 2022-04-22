
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
        public Task<List<Project>> GetAllProjects();
        public Task<int> CreateProject(Project projectDTO);
        //public Task<Project> UpdateProject(Project projectDTO);
    }
}

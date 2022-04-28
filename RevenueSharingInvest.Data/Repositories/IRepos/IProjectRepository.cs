
using RevenueSharingInvest.Data.Models.DTOs;
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
        public Task<int> CreateProject(Project projectDTO);

        //READ
        public Task<List<Project>> GetAllProjects();

        //UPDATE
        public Task<int> UpdateProject(Project projectDTO, Guid projectId);

        //DELETE
        public Task<int> DeleteProjectById(Guid projectId);

    }
}

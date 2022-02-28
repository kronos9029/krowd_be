using AutoMapper;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository projectRepository;


        public ProjectService(IProjectRepository projectRepository)
        {
            this.projectRepository = projectRepository;
        }

        public async Task<List<Project>> GetAllProjects()
        {
            List<Project> projectList = await projectRepository.GetAllProjects();
            return projectList;
        }
    }
}

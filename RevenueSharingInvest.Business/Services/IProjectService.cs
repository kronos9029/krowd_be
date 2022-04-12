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
        public Task<List<Project>> GetAllProjects();
        public Task<int> CreateProject(ProjectDTO newProjectDTO);
    }
}

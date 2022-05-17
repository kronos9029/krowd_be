using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IProjectEntityRepository
    {
        //CREATE
        public Task<int> CreateProjectEntity(ProjectEntity projectEntityDTO);

        //READ
        public Task<List<ProjectEntity>> GetAllProjectEntitys();
        public Task<ProjectEntity> GetProjectEntityById(Guid projectEntityId);

        //UPDATE
        public Task<int> UpdateProjectEntity(ProjectEntity projectEntityDTO, Guid projectEntityId);

        //DELETE
        public Task<int> DeleteProjectEntityById(Guid projectEntityId);
    }
}

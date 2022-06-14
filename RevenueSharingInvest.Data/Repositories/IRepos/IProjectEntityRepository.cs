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
        public Task<string> CreateProjectEntity(ProjectEntity projectEntityDTO);

        //READ
        public Task<List<ProjectEntity>> GetAllProjectEntities(int pageIndex, int pageSize);
        public Task<ProjectEntity> GetProjectEntityById(Guid projectEntityId);

        //UPDATE
        public Task<int> UpdateProjectEntity(ProjectEntity projectEntityDTO, Guid projectEntityId);

        //DELETE
        public Task<int> DeleteProjectEntityById(Guid projectEntityId);
        public Task<int> ClearAllProjectEntityData();
    }
}

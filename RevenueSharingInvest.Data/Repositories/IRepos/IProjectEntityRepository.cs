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
        public Task<int> CreateProjectEntityById(ProjectEntity projectEntityDTO);

        //READ
        //public Task<List<ProjectEntity>> GetAllProjectEntities(int pageIndex, int pageSize);
        public Task<ProjectEntity> GetProjectEntityById(Guid projectEntityId);
        public Task<List<ProjectEntity>> GetProjectEntityByProjectIdAndType(Guid projectId, string type);
        public Task<int> CountProjectEntityByProjectIdAndType(Guid projectId, string type);

        //UPDATE
        public Task<int> UpdateProjectEntity(ProjectEntity projectEntityDTO, Guid projectEntityId);
        public Task<int> UpdateProjectEntityPriority(Guid projectEntityId, int priority, Guid? updaterId);

        //DELETE
        public Task<int> DeleteProjectEntityById(Guid projectId);
        public Task<int> DeleteProjectEntityByProjectIdAndType(Guid projectId, string type);
        public Task<int> DeleteProjectEntityByProjectId(Guid projectId);
        public Task<int> ClearAllProjectEntityData();

        //Firebase
        public Task<int> CreateProjectEntityFromFirebase(ProjectEntity projectEntityDTO);
    }
}

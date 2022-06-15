using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IProjectEntityService
    {
        //CREATE
        public Task<IdDTO> CreateProjectEntity(ProjectEntityDTO projectEntityDTO);

        //READ
        public Task<List<ProjectEntityDTO>> GetAllProjectEntities(int pageIndex, int pageSize);
        public Task<ProjectEntityDTO> GetProjectEntityById(Guid projectEntityId);

        //UPDATE
        public Task<int> UpdateProjectEntity(ProjectEntityDTO projectEntityDTO, Guid projectEntityId);

        //DELETE
        public Task<int> DeleteProjectEntityById(Guid projectEntityId);
        public Task<int> ClearAllProjectEntityData();
    }
}

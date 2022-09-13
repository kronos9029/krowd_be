using RevenueSharingInvest.API;
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
        public Task<IdDTO> CreateProjectEntity(CreateProjectEntityDTO projectEntityDTO, ThisUserObj currentUser);

        //READ
        public Task<List<GetProjectEntityDTO>> GetProjectEntityByProjectIdAndType(Guid projectId, string type, ThisUserObj currentUser);
        public Task<GetProjectEntityDTO> GetProjectEntityById(Guid projectEntityId, ThisUserObj currentUser);

        //UPDATE
        public Task<int> UpdateProjectEntity(UpdateProjectEntityDTO projectEntityDTO, Guid projectEntityId, ThisUserObj currentUser);
        public Task<int> UpdateProjectEntityPriority(List<ProjectEntityUpdateDTO> idList, ThisUserObj currentUser);

        //DELETE
        public Task<int> DeleteProjectEntityById(Guid projectEntityId);
        public Task<int> ClearAllProjectEntityData();
    }
}

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
        public Task<IdDTO> CreateProjectEntity(CreateUpdateProjectEntityDTO projectEntityDTO);

        //READ
        public Task<List<GetProjectEntityDTO>> GetAllProjectEntities(int pageIndex, int pageSize);
        public Task<GetProjectEntityDTO> GetProjectEntityById(Guid projectEntityId);

        //UPDATE
        public Task<int> UpdateProjectEntity(CreateUpdateProjectEntityDTO projectEntityDTO, Guid projectEntityId);
        public Task<int> UpdateProjectEntityPriority(List<string> idList);

        //DELETE
        public Task<int> DeleteProjectEntityById(Guid projectEntityId);
        public Task<int> ClearAllProjectEntityData();
    }
}

using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IAreaRepository
    {
        //CREATE
        public Task<string> CreateArea(Area areaDTO);

        //READ
        public Task<List<Area>> GetAllAreas(int pageIndex, int pageSize);
        public Task<Area> GetAreaById(Guid areaId);

        //UPDATE
        public Task<int> UpdateArea(Area areaDTO, Guid areaId);

        //DELETE
        public Task<int> DeleteAreaById(Guid areaId);
        public Task<int> ClearAllAreaData();
    }
}

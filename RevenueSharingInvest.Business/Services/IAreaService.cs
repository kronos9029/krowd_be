using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IAreaService
    {
        //CREATE
        public Task<IdDTO> CreateArea(CreateUpdateAreaDTO areaDTO, ThisUserObj currentUser);

        //READ
        public Task<List<GetAreaDTO>> GetAllAreas(int pageIndex, int pageSize, ThisUserObj currentUser);
        public Task<GetAreaDTO> GetAreaById(Guid areaId, ThisUserObj currentUser);

        //UPDATE
        public Task<int> UpdateArea(CreateUpdateAreaDTO areaDTO, Guid areaId, ThisUserObj currentUser);

        //DELETE
        public Task<int> DeleteAreaById(Guid areaId, ThisUserObj currentUser);
    }
}

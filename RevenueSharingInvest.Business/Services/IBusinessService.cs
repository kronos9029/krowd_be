using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IBusinessService
    {
        //CREATE
        public Task<IdDTO> CreateBusiness(CreateBusinessDTO businessDTO, List<string> fieldIdList, string creatorId);
        //public Task<int> AdminCreateBusiness(Data.Models.Entities.Business newBusiness, string email);

        //READ
        public Task<AllBusinessDTO> GetAllBusiness(int pageIndex, int pageSize, string status, string name, string orderBy, string order, ThisUserObj currentUser);
        public Task<GetBusinessDTO> GetBusinessById(Guid businessId);
        public Task<Data.Models.Entities.Business> GetBusinessByProjectId(Guid projectId);

        //UPDATE
        public Task<int> UpdateBusiness(UpdateBusinessDTO businessDTO, Guid businessId);
        public Task<int> UpdateBusinessStatus(Guid businessId, String status);

        //DELETE
        public Task<int> DeleteBusinessById(Guid businessId);
        public Task<int> ClearAllBusinessData();
    }
}

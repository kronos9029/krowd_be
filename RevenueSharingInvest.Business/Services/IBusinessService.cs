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
        public Task<IdDTO> CreateBusiness(CreateUpdateBusinessDTO businessDTO, List<string> fieldIdList, string adminId);
        //public Task<int> AdminCreateBusiness(Data.Models.Entities.Business newBusiness, string email);

        //READ
        public Task<AllBusinessDTO> GetAllBusiness(int pageIndex, int pageSize, string? orderBy, string? order, string temp_field_role);
        public Task<GetBusinessDTO> GetBusinessById(Guid businessId);

        //UPDATE
        public Task<int> UpdateBusiness(CreateUpdateBusinessDTO businessDTO, Guid businessId);
        public Task<int> UpdateBusinessStatus(Guid businessId, String status);

        //DELETE
        public Task<int> DeleteBusinessById(Guid businessId);
        public Task<int> ClearAllBusinessData();
    }
}

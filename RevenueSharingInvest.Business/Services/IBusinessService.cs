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
        public Task<int> CreateBusiness(BusinessDTO businessDTO);
        //public Task<int> AdminCreateBusiness(Data.Models.Entities.Business newBusiness, string email);

        //READ
        public Task<List<BusinessDTO>> GetAllBusiness();
        public Task<BusinessDTO> GetBusinessById(Guid businessId);

        //UPDATE
        public Task<int> UpdateBusiness(BusinessDTO businessDTO, Guid businessId);

        //DELETE
        public Task<int> DeleteBusinessById(Guid businessId);
    }
}

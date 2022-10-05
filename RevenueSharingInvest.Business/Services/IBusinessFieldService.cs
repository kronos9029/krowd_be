using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IBusinessFieldService
    {
        //CREATE
        //public Task<int> CreateBusinessField(BusinessFieldDTO businessFieldDTO);

        //READ
        public Task<List<BusinessFieldDTO>> GetAllBusinessFields(int pageIndex, int pageSize);
        public Task<BusinessFieldDTO> GetBusinessFieldById(Guid businessId, Guid fieldId);

        //UPDATE

        //DELETE
        //public Task<int> DeleteBusinessFieldById(Guid businessId, Guid fieldId);
        //public Task<int> ClearAllBusinessFieldData();
    }
}

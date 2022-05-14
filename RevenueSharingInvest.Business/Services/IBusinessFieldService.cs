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
        public Task<int> CreateBusinessField(BusinessFieldDTO businessFieldDTO);

        //READ
        public Task<List<BusinessFieldDTO>> GetAllBusinessFields();
        public Task<BusinessFieldDTO> GetBusinessFieldById(Guid businessFieldId);

        //UPDATE
        public Task<int> UpdateBusinessField(BusinessFieldDTO businessFieldDTO, Guid businessFieldId);

        //DELETE
        public Task<int> DeleteBusinessFieldById(Guid businessFieldId);
    }
}

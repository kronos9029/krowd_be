using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IFieldService
    {
        //CREATE
        public Task<IdDTO> CreateField(FieldDTO fieldDTO);

        //READ
        public Task<List<FieldDTO>> GetAllFields(int pageIndex, int pageSize);
        public Task<FieldDTO> GetFieldById(Guid fieldId);

        //UPDATE
        public Task<int> UpdateField(FieldDTO fieldDTO, Guid fieldId);

        //DELETE
        public Task<int> DeleteFieldById(Guid fieldId);
        public Task<int> ClearAllFieldData();
    }
}

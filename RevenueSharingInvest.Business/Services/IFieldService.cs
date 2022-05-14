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
        public Task<int> CreateField(FieldDTO fieldDTO);

        //READ
        public Task<List<FieldDTO>> GetAllFields();
        public Task<FieldDTO> GetFieldById(Guid fieldId);

        //UPDATE
        public Task<int> UpdateField(FieldDTO fieldDTO, Guid fieldId);

        //DELETE
        public Task<int> DeleteFieldById(Guid fieldId);
    }
}

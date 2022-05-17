using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IFieldRepository
    {
        //CREATE
        public Task<int> CreateField(Field fieldDTO);

        //READ
        public Task<List<Field>> GetAllFields();
        public Task<Field> GetFieldById(Guid fieldId);

        //UPDATE
        public Task<int> UpdateField(Field fieldDTO, Guid fieldId);

        //DELETE
        public Task<int> DeleteFieldById(Guid fieldId);
    }
}

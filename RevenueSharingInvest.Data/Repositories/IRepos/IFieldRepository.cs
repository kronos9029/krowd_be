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
        public Task<string> CreateField(Field fieldDTO);

        //READ
        public Task<List<Field>> GetAllFields(int pageIndex, int pageSize);
        public Task<Field> GetFieldById(Guid fieldId);
        public Task<List<Field>> GetCompanyFields(Guid businessId);
        public Task<Field> GetProjectFieldByProjectId(Guid projectId);

        //UPDATE
        public Task<int> UpdateField(Field fieldDTO, Guid fieldId);

        //DELETE
        public Task<int> DeleteFieldById(Guid fieldId);
        public Task<int> ClearAllFieldData();
    }
}

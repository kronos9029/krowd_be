using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.CommonRepos
{
    public interface IValidationRepository
    {
        public Task<dynamic> CheckExistenceId(string tableName, Guid id);
    }
}

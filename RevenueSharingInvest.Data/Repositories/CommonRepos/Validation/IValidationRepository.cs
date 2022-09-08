using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.ExtensionsRepos
{
    public interface IValidationRepository
    {
        public Task<dynamic> CheckExistenceId(string tableName, Guid id);
        public Task<dynamic> CheckExistenceUserWithRole(Guid id);
    }
}

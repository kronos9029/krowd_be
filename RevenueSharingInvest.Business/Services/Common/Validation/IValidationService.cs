using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Common
{
    public interface IValidationService
    {
        public Task<bool> CheckExistenceId(string tableName, Guid id);
        public Task<bool> CheckExistenceUserWithRole(string role, Guid id);
        public Task<bool> CheckUUIDFormat(string uuid);
        public Task<bool> CheckEmail(string email);
        public Task<bool> CheckDate(string date);
        public Task<bool> CheckPhoneNumber(string phoneNum);
        public Task<bool> CheckInt(int number);
        public Task<bool> CheckText(string text);
    }
}

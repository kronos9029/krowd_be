
using AutoMapper;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.ExtensionsRepos;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions
{
    public class ValidationService : IValidationService
    {
        private readonly IMapper _mapper;
        private readonly IBusinessFieldRepository _businessFieldRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidationRepository _validationRepository;
        private readonly Regex regexMail = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        private readonly Regex regexPhone = new(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");
        private readonly Regex regexDOB = new(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$");
        private readonly Regex regexDate = new(@"^([1-9]|([012][0-9])|(3[01]))/([0]{0,1}[1-9]|1[012])/\d\d\d\d (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d$"); //[dd/MM/yyyy HH:mm:ss]
        private readonly Regex regexUUID = new(@"(?im)^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$");

        public ValidationService(IMapper mapper, IBusinessFieldRepository businessFieldRepository, IUserRepository userRepository, IValidationRepository validationRepository)
        {
            _mapper = mapper;
            _businessFieldRepository = businessFieldRepository;
            _userRepository = userRepository;
            _validationRepository = validationRepository;
        }

        public async Task<bool> CheckDate(string date)
        {
            bool result;
            try
            {
                Match match = regexDate.Match(date);
                result = (date.Length == 0) ? false : match.Success;
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> CheckDOB(string dob)
        {
            bool result;
            try
            {
                Match match = regexDOB.Match(dob);
                result = (dob.Length == 0) ? false : match.Success;
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> CheckEmail(string email)
        {
            bool result;
            try
            {
                Match match = regexMail.Match(email);
                result = (email.Length == 0) ? false : match.Success;
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> CheckExistenceId(string tableName, Guid id)
        {
            bool result;
            try
            {
                dynamic isExisted = await _validationRepository.CheckExistenceId(tableName, id);
                result = (isExisted != null) ? true : false;
                return result;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> CheckExistenceUserWithRole(string role, Guid id)
        {
            try
            {
                dynamic isExisted = await _validationRepository.CheckExistenceUserWithRole(id);
                if(isExisted != null)
                {
                    User dto = _mapper.Map<User>(isExisted);
                    return (dto.RoleId.Equals(Guid.Parse(role))) ? true : false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<bool> CheckInt(int number)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckManagerOfBusiness(Guid managerId, Guid businessId)
        {
            bool result;           
            try
            {
                User manager = await _userRepository.GetUserById(managerId);
                result = (!businessId.Equals(manager.BusinessId)) ? false : true;
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> CheckPhoneNumber(string phoneNum)
        {
            bool result;
            try
            {
                Match match = regexPhone.Match(phoneNum);
                result = (phoneNum.Length == 0) ? false : match.Success;
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> CheckProjectFieldInBusinessField(Guid businessId, Guid fieldId)
        {
            bool result;
            try
            {
                result = (await _businessFieldRepository.GetBusinessFieldById(businessId, fieldId) == null) ? false : true;
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> CheckText(string text)
        {
            bool result;
            try
            {
                result = (text == null || text.Length == 0 || text.Equals("string")) ? false : true;
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> CheckUUIDFormat(string uuid)
        {
            bool result;
            try
            {
                Match match = regexUUID.Match(uuid);
                result = (uuid.Length == 0) ? false : match.Success;
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> FormatDateInput(string dateString)
        {
            try
            {
                return DateTime.ParseExact(dateString, "dd/MM/yyyy HH:mm:ss", null).ToString("MM/dd/yyyy HH:mm:ss");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> FormatDateOutput(string dateString)
        {
            try
            {
                return DateTime.Parse(dateString).ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

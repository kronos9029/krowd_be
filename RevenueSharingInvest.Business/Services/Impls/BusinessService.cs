using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IUserRepository _userRepository;
        private readonly String ROLE_ADMIN_ID = "";

        public BusinessService(IBusinessRepository businessRepository, IUserRepository userRepository)
        {
            _businessRepository = businessRepository;
            _userRepository = userRepository;
        }
        public async Task<int> AdminCreateBusiness(Data.Models.Entities.Business newBusiness, string email)
        {
            User userObject = await _userRepository.GetUserByEmail(email);
            if(userObject == null)
            {
                throw new NotFoundException("User Not Found!!");
            }
            else
            {
                if (!userObject.RoleId.ToString().Equals(ROLE_ADMIN_ID))
                {
                    throw new Exceptions.UnauthorizedAccessException("Only Admin Can Create Business!!");
                }
                else
                {
                    if(await _businessRepository.CreateBusiness(newBusiness) < 1)
                    {
                        throw new CreateBusinessException("Create Business Fail!!");
                    }
                }
            }

            return 1;
        }


    }
}

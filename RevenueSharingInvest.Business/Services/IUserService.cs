using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IUserService
    {
        public Task<AuthenticateResponse> GetTokenInvestor(string firebaseToken);
        public Task<AuthenticateResponse> GetTokenWebBusiness(string firebaseToken);
        /*public Task<int> AdminCreateBusiness(User newBusiness, string email);*/

    }
}

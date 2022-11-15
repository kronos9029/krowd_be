using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IAuthenticateService
    {
        public Task<AuthenticateResponse> GetTokenInvestor(string firebaseToken, string deviceToken);
        public Task<AuthenticateResponse> GetTokenWebBusiness(string firebaseToken);
        public Task<AuthenticateResponse> GetTokenAdmin(string firebaseToken);
        public Task<AuthenticateResponse> GetTokenProjectManager(string firebaseToken);
    }
}

using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;

        public UserService(IOptions<AppSettings> appSettings, IUserRepository userRepository)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
        }

        public async Task<AuthenticateResponse> GetTokenMobileInvestor(string firebaseToken)
        {
            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            AuthenticateResponse authenResponse = new AuthenticateResponse();
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            DateTime createdDate = (DateTime)userRecord.UserMetaData.CreationTimestamp;
            string ImageUrl = userRecord.PhotoUrl.ToString();

            User userObject = await _userRepository.GetUserByEmail(email);


            if (userObject == null)
            {
                int checkCreate = await _userRepository.CreateUser(email, createdDate, ImageUrl, null, null, null);
                if (checkCreate > 0)
                {
                    if (checkCreate == 0)
                    {
                        throw new RegisterException(" Register Fail!!");
                    }



                }

            }
            else
            {

            }
            return null;
        }

        private AuthenticateResponse GenerateToken(AuthenticateResponse response, string roleCheck)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            Claim roleClaim;

            if (roleCheck.Equals(RoleEnum.Admin.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.Admin.ToString());
            }
            else if (roleCheck.Equals(RoleEnum.Business.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.Business.ToString());
            }
            else if (roleCheck.Equals(RoleEnum.Investor.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.Investor.ToString());
            }
            else
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.Nigga.ToString());
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                   new Claim(ClaimTypes.GivenName, response.uid),
                   new Claim(ClaimTypes.SerialNumber, response.id.ToString()),
                   roleClaim
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            response.token = tokenHandler.WriteToken(token);
            return response;
        }
    }
}

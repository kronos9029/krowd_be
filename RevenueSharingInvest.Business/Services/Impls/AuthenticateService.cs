using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Common;
using RevenueSharingInvest.Data.Models.Constants;
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
    public class AuthenticateService : IAuthenticateService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly IInvestorRepository _investorRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public AuthenticateService(IOptions<AppSettings> appSettings, 
            IUserRepository userRepository, 
            IInvestorRepository investorRepository, 
            IValidationService validationService,
            IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _investorRepository = investorRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<AuthenticateResponse> GetTokenInvestor(string firebaseToken)
        {
            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            string lastName = userRecord.DisplayName;
            DateTime createdDate = (DateTime)userRecord.UserMetaData.CreationTimestamp;
            string ImageUrl = userRecord.PhotoUrl.ToString();

            User userObject = await _userRepository.GetUserByEmail(email);


            AuthenticateResponse response = new();

            if (userObject == null)
            {

                Guid roleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.INVESTOR.ToString()));

                Investor investor = new();
                User newInvestorUser = new();

                newInvestorUser.Email = email;
                newInvestorUser.CreateDate = createdDate;
                newInvestorUser.Image = ImageUrl;
                newInvestorUser.RoleId = roleId;
                newInvestorUser.LastName = lastName;

                string newUserID = await _userRepository.CreateUser(newInvestorUser);
                if (newUserID.Equals(""))
                {
                    throw new RegisterException("Register Fail!!");
                }

                investor.UserId = Guid.Parse(newUserID);
                //investor.InvestorTypeId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.INVESTOR.ToString()));

                string newInvestorID = await _investorRepository.CreateInvestor(investor);
                if (newInvestorID.Equals("")) 
                {
                    throw new RegisterException("Create Investor Fail!!"); 
                }
                response.email = email;
                response.id = Guid.Parse(newUserID);
                response.uid = uid;
                response.investorId = Guid.Parse(newInvestorID);
                response.image = ImageUrl;
                response = await GenerateTokenAsync(response, RoleEnum.INVESTOR.ToString());
            }
            else
            {
                response.email = email;
                response.id = userObject.Id;
                response.uid = uid;
                response.image = ImageUrl;
                response.investorId = await _investorRepository.GetInvestorByEmail(email);
                response = await GenerateTokenAsync(response, RoleEnum.INVESTOR.ToString());
            }
            return response;
        }

        public async Task<AuthenticateResponse> GetTokenWebBusiness(string firebaseToken)
        {
            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;

            User userObject = await _userRepository.GetUserByEmail(email);

            AuthenticateResponse response = new();

            if (userObject == null)
            {
                throw new NotFoundException("User Not Found!!");
            }
            else
            {
                response.email = email;
                response.id = userObject.Id;
                response.uid = uid;
                response.businessId = userObject.BusinessId;
                response.roleId = userObject.RoleId;
                response.roleName = RoleEnum.BUSINESS_MANAGER.ToString();
                response.image = userObject.Image;
                response.fullName = userObject.FirstName + " " + userObject.LastName;
                response = await GenerateTokenAsync(response, RoleEnum.BUSINESS_MANAGER.ToString());
            }

            return response;
        }

        public async Task<AuthenticateResponse> GetTokenProjectManager(string firebaseToken)
        {
            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;

            User userObject = await _userRepository.GetUserByEmail(email);

            AuthenticateResponse response = new();

            if (userObject == null)
            {
                throw new NotFoundException("User Not Found!!");
            }
            else
            {
                response.email = email;
                response.id = userObject.Id;
                response.uid = uid;
                response.businessId = userObject.BusinessId;
                response = await GenerateTokenAsync(response, RoleEnum.PROJECT_MANAGER.ToString());
            }

            return response;
        }

        public async Task<AuthenticateResponse> GetTokenAdmin(string firebaseToken)
        {
            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;


            AuthenticateResponse response = new();

            response.email = email;
            response.uid = uid;
            response =  await GenerateTokenAsync(response, RoleEnum.ADMIN.ToString());


            return response;
        }

        private async Task<AuthenticateResponse> GenerateTokenAsync(AuthenticateResponse response, string roleCheck)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            Claim roleClaim, investorId;

            if (roleCheck.Equals(RoleEnum.ADMIN.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.ADMIN.ToString());
                investorId = new Claim(ClaimTypes.GroupSid, "");

            }
            else if (roleCheck.Equals(RoleEnum.INVESTOR.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.INVESTOR.ToString());
                investorId = new Claim(ClaimTypes.GroupSid, response.investorId.ToString());

            }
            else if (roleCheck.Equals(RoleEnum.BUSINESS_MANAGER.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.BUSINESS_MANAGER.ToString());
                investorId = new Claim(ClaimTypes.GroupSid, "");

            }
            else if (roleCheck.Equals(RoleEnum.PROJECT_MANAGER.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.PROJECT_MANAGER.ToString());
                investorId = new Claim(ClaimTypes.GroupSid, "");

            }
            else
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.INVESTOR.ToString());
                investorId = new Claim(ClaimTypes.GroupSid, response.investorId.ToString());

            }

            int hours;

            if (roleCheck.Equals(RoleEnum.ADMIN.ToString()))
            {
                hours = 8760;
            }
            else
            {
                hours = 8760;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                   new Claim(ClaimTypes.SerialNumber, response.id.ToString()),
                   new Claim(ClaimTypes.Email, response.email),
                   investorId,
                   roleClaim,
                }),

                Expires = DateTime.UtcNow.AddHours(hours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);


            string storage = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(response.uid);

            response.token = tokenHandler.WriteToken(token);
            return response;
        }


    }
}

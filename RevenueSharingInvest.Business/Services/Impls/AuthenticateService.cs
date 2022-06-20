using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Common;
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
        private readonly IBusinessRepository _businessRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IInvestorService _investorService;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly String ROLE_ADMIN_ID = "ff54acc6-c4e9-4b73-a158-fd640b4b6940";
        private readonly String ROLE_INVESTOR_ID = "ad5f37da-ca48-4dc5-9f4b-963d94b535e6";
        private readonly String ROLE_BUSINESS_MANAGER_ID = "015ae3c5-eee9-4f5c-befb-57d41a43d9df";
        private readonly String ROLE_PROJECT_OWNER_ID = "2d80393a-3a3d-495d-8dd7-f9261f85cc8f";
        private readonly String INVESTOR_TYPE_ID = "";

        public AuthenticateService(IOptions<AppSettings> appSettings, IUserRepository userRepository, IInvestorRepository investorRepository, IValidationService validationService, IMapper mapper)
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
            DateTime createdDate = (DateTime)userRecord.UserMetaData.CreationTimestamp;
            string ImageUrl = userRecord.PhotoUrl.ToString();

            User userObject = await _userRepository.GetUserByEmail(email);

            AuthenticateResponse response = new();

            if (userObject == null)
            {
                Role role = await _roleRepository.GetRoleById(Guid.Parse(ROLE_INVESTOR_ID));

                Investor investor = new();
                User newInvestorObject = new();

                newInvestorObject.Email = email;
                newInvestorObject.CreateDate = createdDate;
                newInvestorObject.Image = ImageUrl;
                newInvestorObject.RoleId = role.Id;

                string newUserID = await _userRepository.CreateUser(newInvestorObject);
                if (newUserID.Equals(""))
                {
                    throw new RegisterException("Register Fail!!");
                }

                investor.UserId = Guid.Parse(newUserID);
                investor.InvestorTypeId = Guid.Parse(INVESTOR_TYPE_ID);

                string newInvestorID = await _investorRepository.CreateInvestor(investor);
                if (newInvestorID.Equals("")) 
                {
                    throw new RegisterException("Create Investor Fail!!"); 
                }
                response.email = email;
                response.id = Guid.Parse(newUserID);
                response.uid = uid;
                response.investorId = Guid.Parse(newInvestorID);
                response = GenerateToken(response, RoleEnum.INVESTOR.ToString());
            }
            else
            {
                response.email = email;
                response.id = userObject.Id;
                response.uid = uid;
                response = GenerateToken(response, RoleEnum.INVESTOR.ToString());
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
                throw new NotFoundException("Business Not Found!!");
            }
            else
            {
                response.email = email;
                response.id = userObject.Id;
                response.uid = uid;
                response = GenerateToken(response, RoleEnum.BUSINESS_MANAGER.ToString());
            }

            return response;
        }

        private AuthenticateResponse GenerateToken(AuthenticateResponse response, string roleCheck)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            Claim roleClaim, roleId;

            if (roleCheck.Equals(RoleEnum.ADMIN.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.ADMIN.ToString());
                roleId = new Claim(ClaimTypes.AuthenticationInstant, ROLE_ADMIN_ID);
            }
            else if (roleCheck.Equals(RoleEnum.INVESTOR.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.INVESTOR.ToString());
                roleId = new Claim(ClaimTypes.AuthenticationInstant, ROLE_INVESTOR_ID);
            }
            else if (roleCheck.Equals(RoleEnum.BUSINESS_MANAGER.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.BUSINESS_MANAGER.ToString());
                roleId = new Claim(ClaimTypes.AuthenticationInstant, ROLE_BUSINESS_MANAGER_ID);
            }            
            else if (roleCheck.Equals(RoleEnum.PROJECT_OWNER.ToString()))
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.PROJECT_OWNER.ToString());
                roleId = new Claim(ClaimTypes.AuthenticationInstant, ROLE_PROJECT_OWNER_ID);
            }
            else
            {
                roleClaim = new Claim(ClaimTypes.Role, RoleEnum.INVESTOR.ToString());
                roleId = new Claim(ClaimTypes.AuthenticationInstant, ROLE_INVESTOR_ID);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                   new Claim(ClaimTypes.SerialNumber, response.id.ToString()),
                   roleId,
                   roleClaim
                }),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            response.token = tokenHandler.WriteToken(token);
            return response;
        }

        public async Task<bool> CheckRoleForUser(String userId, String requiredRole)
        {
         /* if (!await _validationService.CheckExistenceId("[User]", Guid.Parse(userId)))
                throw new NotFoundException("concac");*/
            User userObj = await _userRepository.GetUserById(Guid.Parse(userId));

            Role role = await _roleRepository.GetRoleByName(requiredRole);

            if(role == null){
                throw new NotFoundException("No Role Found!!");
            }
            else
            {
                if (userObj.RoleId.ToString().Equals(role.Id.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}

using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.RedisCache;
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
using UnauthorizedAccessException = RevenueSharingInvest.Business.Exceptions.UnauthorizedAccessException;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly IInvestorRepository _investorRepository;
        private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public AuthenticateService(IOptions<AppSettings> appSettings, 
            IUserRepository userRepository, 
            IInvestorRepository investorRepository,
            IInvestorWalletRepository investorWalletRepository,
            IValidationService validationService,
            IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _investorRepository = investorRepository;
            _investorWalletRepository = investorWalletRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<AuthenticateResponse> GetTokenInvestor(string firebaseToken, string deviceToken)
        {
            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            string lastName = userRecord.DisplayName;
            string ImageUrl = userRecord.PhotoUrl.ToString();

            User userObject = await _userRepository.GetUserByEmail(email);



            AuthenticateResponse response = new();

            if (userObject == null)
            {
                Guid roleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.INVESTOR.ToString()));

                Investor investor = new();
                User newInvestorUser = new()
                {
                    Email = email,
                    Image = ImageUrl,
                    RoleId = roleId,
                    LastName = lastName,
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    DeviceToken = deviceToken ?? ""
                };

                //Tạo User object Role INVESTOR
                string newUserID = await _userRepository.CreateUser(newInvestorUser);
                if (newUserID.Equals(""))
                {
                    throw new RegisterException("Register Fail!!");
                }

                //Tạo Investor object
                investor.UserId = Guid.Parse(newUserID);
                string newInvestorID = await _investorRepository.CreateInvestor(investor);
                if (newInvestorID.Equals("")) 
                {
                    throw new RegisterException("Create Investor Fail!!"); 
                }
                else
                {
                    //Tạo ví I1, I2, I3, I4, I5
                    await _investorWalletRepository.CreateInvestorWallet(Guid.Parse(newInvestorID), Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I1")), Guid.Parse(newUserID));
                    await _investorWalletRepository.CreateInvestorWallet(Guid.Parse(newInvestorID), Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I2")), Guid.Parse(newUserID));
                    await _investorWalletRepository.CreateInvestorWallet(Guid.Parse(newInvestorID), Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I3")), Guid.Parse(newUserID));
                    await _investorWalletRepository.CreateInvestorWallet(Guid.Parse(newInvestorID), Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I4")), Guid.Parse(newUserID));
                    await _investorWalletRepository.CreateInvestorWallet(Guid.Parse(newInvestorID), Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("I5")), Guid.Parse(newUserID));
                }
                response.email = email;
                response.id = Guid.Parse(newUserID);
                response.uid = uid;
                response.investorId = Guid.Parse(newInvestorID);
                response.image = ImageUrl;
                response.fullName = userRecord.DisplayName;
                response = await GenerateTokenAsync(response, RoleEnum.INVESTOR.ToString());
            }
            else
            {
                if (!userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.INVESTOR.ToString())))
                    throw new RegisterException("This Is Not An Investor Email!!");
                if(deviceToken != null || !deviceToken.Equals(""))
                {
                    if (!userObject.DeviceToken.Equals(deviceToken))
                    {
                        await _userRepository.UpdateDeviceToken(deviceToken, userObject.Id);
                    }
                }

                response.email = email;
                response.id = userObject.Id;
                response.uid = uid;
                response.image = userObject.Image;
                response.fullName = (userObject.FirstName + " " + userObject.LastName) ?? userRecord.DisplayName;
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
            string ImageUrl = userRecord.PhotoUrl.ToString();

            User userObject = await _userRepository.GetUserByEmail(email);
            AuthenticateResponse response = new();
            
            if (userObject == null)
            {
                throw new NotFoundException("User Not Found!!");
            }

            if (userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.BUSINESS_MANAGER.ToString())) 
                || userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.PROJECT_MANAGER.ToString())))
            {
                bool isBusinessManager = userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.BUSINESS_MANAGER.ToString())) ? true : false;

                response.email = email;
                response.id = userObject.Id;
                response.uid = uid;
                response.businessId = userObject.BusinessId;
                response.roleId = userObject.RoleId;
                response.roleName = isBusinessManager ? RoleEnum.BUSINESS_MANAGER.ToString() : RoleEnum.PROJECT_MANAGER.ToString();
                response.image = userObject.Image ?? ImageUrl;
                response.fullName = userObject.FirstName + " " + userObject.LastName;
                response = await GenerateTokenAsync(response, response.roleName);

                return response;
                
            } else
            {
                throw new UnauthorizedAccessException("You Are Not Using An Business/Project Manager Account!!");
            }
            
        }

        public async Task<AuthenticateResponse> GetTokenProjectManager(string firebaseToken)
        {
            FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
            string uid = decryptedToken.Uid;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            string ImageUrl = userRecord.PhotoUrl.ToString();

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
                response.image = userObject.Image ?? ImageUrl;
                response.roleId = userObject.RoleId;
                response.roleName = RoleEnum.PROJECT_MANAGER.ToString();
                response.fullName = userObject.FirstName + " " + userObject.LastName;
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
            string ImageUrl = userRecord.PhotoUrl.ToString();


            User userObject = await _userRepository.GetUserByEmail(email);
            
            if (userObject == null)
                throw new NotFoundException("User Not Found!!");

            if (!userObject.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString())))
            {
                throw new UnauthorizedAccessException("You Are Not Using An Admin Account!!");
            }

            AuthenticateResponse response = new()
            {
                email = email,
                uid = uid,
                id = userObject.Id,
                image = userObject.Image ?? ImageUrl,
                fullName = (userObject.FirstName + " " + userObject.LastName) ?? userRecord.DisplayName
            };
            response = await GenerateTokenAsync(response, RoleEnum.ADMIN.ToString());


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
                   new Claim(ClaimTypes.Actor, response.fullName),
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

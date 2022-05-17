﻿using AutoMapper;
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
        private readonly IInvestorRepository _investorRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly String ROLE_ADMIN_ID = "";
        private readonly String ROLE_INVESTOR_ID = "";
        private readonly String ROLE_BUSINESS_MANAGER_ID = "";
        private readonly String ROLE_PROJECT_OWNER_ID = "";
        private readonly String INVESTOR_TYPE_ID = "";

        public UserService(IOptions<AppSettings> appSettings, IUserRepository userRepository, IInvestorRepository investorRepository, IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _investorRepository = investorRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateUser(UserDTO userDTO)
        {
            int result;
            try
            {
                User dto = _mapper.Map<User>(userDTO);
                result = await _userRepository.CreateUser(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create User Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteUserById(Guid userId)
        {
            int result;
            try
            {

                result = await _userRepository.DeleteUserById(userId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete User Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<UserDTO>> GetAllUsers()
        {
            List<User> userList = await _userRepository.GetAllUsers();
            List<UserDTO> list = _mapper.Map<List<UserDTO>>(userList);
            return list;
        }

        //GET BY ID
        public async Task<UserDTO> GetUserById(Guid userId)
        {
            UserDTO result;
            try
            {

                User dto = await _userRepository.GetUserById(userId);
                result = _mapper.Map<UserDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No User Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateUser(UserDTO userDTO, Guid userId)
        {
            int result;
            try
            {
                User dto = _mapper.Map<User>(userDTO);
                result = await _userRepository.UpdateUser(dto, userId);
                if (result == 0)
                    throw new CreateObjectException("Can not update User Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //Tấn Phát coi lại chổ này
        //public async Task<AuthenticateResponse> GetTokenInvestor(string firebaseToken)
        //{
        //    FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
        //    string uid = decryptedToken.Uid;

        //    UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        //    string email = userRecord.Email;
        //    DateTime createdDate = (DateTime)userRecord.UserMetaData.CreationTimestamp;
        //    string ImageUrl = userRecord.PhotoUrl.ToString();

        //    User userObject = await _userRepository.GetUserByEmail(email);

        //    AuthenticateResponse response = new();

        //    if (userObject == null)
        //    {
        //        Role role = await _roleRepository.GetRoleById(Guid.Parse(ROLE_INVESTOR_ID));

        //        Guid userId = Guid.NewGuid();
        //        Guid investorId = Guid.NewGuid();

        //        Investor investor = new();
        //        User newInvestorObject = new();

        //        newInvestorObject.Email = email;
        //        newInvestorObject.CreateDate = createdDate;
        //        newInvestorObject.Image = ImageUrl;
        //        newInvestorObject.RoleId = role.Id;

        //        int checkCreateUser = await _userRepository.CreateInvestorUser(newInvestorObject);
        //        if (checkCreateUser == 0)
        //        {
        //            throw new RegisterException("Register Fail!!");
        //        }
        //        User user = await _userRepository.GetUserByEmail(email);
        //        investor.Id = investorId;
        //        investor.UserId = userId;
        //        investor.InvestorTypeId = Guid.Parse(INVESTOR_TYPE_ID);

        //        int checkCreateInvestor = await _investorRepository.CreateInvestor(investor);
        //        if (checkCreateInvestor == 0)
        //        {
        //            throw new RegisterException("Create Investor Fail!!");
        //        }
        //        response.email = email;
        //        response.id = userId;
        //        response.uid = uid;
        //        response = GenerateToken(response, RoleEnum.Investor.ToString());
        //    }
        //    else
        //    {
        //        response.email = email;
        //        response.id = userObject.Id;
        //        response.uid = uid;
        //        response = GenerateToken(response, RoleEnum.Investor.ToString());
        //    }
        //    return response;
        //}

        //public async Task<AuthenticateResponse> GetTokenWebBusiness(string firebaseToken)
        //{
        //    FirebaseToken decryptedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
        //    string uid = decryptedToken.Uid;

        //    UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        //    string email = userRecord.Email;

        //    User userObject = await _userRepository.GetUserByEmail(email);

        //    AuthenticateResponse response = new();

        //    if (userObject == null)
        //    {
        //        throw new NotFoundException("Business Not Found!!");
        //    }
        //    else
        //    {
        //        response.email = email;
        //        response.id = userObject.Id;
        //        response.uid = uid;
        //        response = GenerateToken(response, RoleEnum.BusinessManager.ToString());
        //    }

        //    return response;
        //}





        //private AuthenticateResponse GenerateToken(AuthenticateResponse response, string roleCheck)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

        //    Claim roleClaim;

        //    if (roleCheck.Equals(RoleEnum.Admin.ToString()))
        //    {
        //        roleClaim = new Claim(ClaimTypes.Role, RoleEnum.Admin.ToString());
        //    }
        //    else if (roleCheck.Equals(RoleEnum.Investor.ToString()))
        //    {
        //        roleClaim = new Claim(ClaimTypes.Role, RoleEnum.Investor.ToString());
        //    }
        //    else if (roleCheck.Equals(RoleEnum.BusinessManager.ToString()))
        //    {
        //        roleClaim = new Claim(ClaimTypes.Role, RoleEnum.BusinessManager.ToString());
        //    }
        //    else
        //    {
        //        roleClaim = new Claim(ClaimTypes.Role, RoleEnum.Investor.ToString());
        //    }

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //           new Claim(ClaimTypes.SerialNumber, response.id.ToString()),
        //           roleClaim
        //        }),
        //        Expires = DateTime.UtcNow.AddDays(7),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    response.token = tokenHandler.WriteToken(token);
        //    return response;
        //}

/*        protected string GenerateOtp()
        {
            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new();
            for (int i = 0; i < 4; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString())) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
        }*/

        private string GenerateOTP(Guid UserId)
        {
            string[] OTP = UserId.ToString().Split("-");

            return OTP[0];
        }

        public async Task<bool> VerifyOTP(String OTP, String email)
        {
            User userObj = await _userRepository.GetUserByEmail(email);

            if (userObj.Id.ToString().Contains(OTP))
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

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
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly IInvestorRepository _investorRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly String ROLE_ADMIN_ID = "ff54acc6-c4e9-4b73-a158-fd640b4b6940";
        private readonly String ROLE_INVESTOR_ID = "ad5f37da-ca48-4dc5-9f4b-963d94b535e6";
        private readonly String ROLE_BUSINESS_MANAGER_ID = "015ae3c5-eee9-4f5c-befb-57d41a43d9df";
        private readonly String ROLE_PROJECT_OWNER_ID = "2d80393a-3a3d-495d-8dd7-f9261f85cc8f";
        private readonly String INVESTOR_TYPE_ID = "";

        public UserService(IOptions<AppSettings> appSettings, IUserRepository userRepository, IInvestorRepository investorRepository, IValidationService validationService, IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _investorRepository = investorRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllUserData()
        {
            int result;
            try 
            {
                result = await _userRepository.ClearAllUserData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateUser(UserDTO userDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (userDTO.roleId == null || !await _validationService.CheckUUIDFormat(userDTO.roleId))
                    throw new InvalidFieldException("Invalid roleId!!!");

                if (!userDTO.roleId.Equals(ROLE_BUSINESS_MANAGER_ID)
                    && !userDTO.roleId.Equals(ROLE_PROJECT_OWNER_ID)
                    && !userDTO.roleId.Equals(ROLE_ADMIN_ID)
                    && !userDTO.roleId.Equals(ROLE_INVESTOR_ID))
                    throw new NotFoundException("This RoleId is not existed!!!");

                if (userDTO.roleId.Equals(ROLE_BUSINESS_MANAGER_ID) || userDTO.roleId.Equals(ROLE_PROJECT_OWNER_ID))
                {
                    if (userDTO.businessId == null)
                        throw new InvalidFieldException("BusinessId is required for BUSINESS_MANAGER or PROJECT_OWNER!!!");

                    if (!await _validationService.CheckUUIDFormat(userDTO.businessId))
                        throw new InvalidFieldException("Invalid businessId!!!");

                    if (!await _validationService.CheckExistenceId("Business", Guid.Parse(userDTO.businessId)))
                        throw new NotFoundException("This BusinessId is not existed!!!");
                }
                else
                {
                    userDTO.businessId = null;
                }               

                if (userDTO.description != null && (userDTO.description.Equals("string") || userDTO.description.Length == 0))
                    userDTO.description = null;

                if (!await _validationService.CheckText(userDTO.lastName))
                    throw new InvalidFieldException("Invalid lastName!!!");

                if (!await _validationService.CheckText(userDTO.firstName))
                    throw new InvalidFieldException("Invalid firstName!!!");

                if (userDTO.image != null && (userDTO.image.Equals("string") || userDTO.image.Length == 0))
                    userDTO.image = null;

                if (userDTO.phoneNum == null || userDTO.phoneNum.Length == 0 || !await _validationService.CheckPhoneNumber(userDTO.phoneNum))
                    throw new InvalidFieldException("Invalid phoneNum!!!");

                if (!await _validationService.CheckText(userDTO.idCard))
                    throw new InvalidFieldException("Invalid idCard!!!");

                if (userDTO.email == null || userDTO.email.Length == 0 || !await _validationService.CheckEmail(userDTO.email))
                    throw new InvalidFieldException("Invalid email!!!");

                if (!await _validationService.CheckText(userDTO.gender))
                    throw new InvalidFieldException("Invalid gender!!!");

                if (userDTO.dateOfBirth == null || userDTO.dateOfBirth.Length == 0 || !await _validationService.CheckDate(userDTO.dateOfBirth))
                    throw new InvalidFieldException("Invalid dateOfBirth!!!");

                if (!await _validationService.CheckText(userDTO.taxIdentificationNumber))
                    throw new InvalidFieldException("Invalid taxIdentificationNumber!!!");

                if (!await _validationService.CheckText(userDTO.city))
                    throw new InvalidFieldException("Invalid city!!!");

                if (!await _validationService.CheckText(userDTO.district))
                    throw new InvalidFieldException("Invalid district!!!");

                if (!await _validationService.CheckText(userDTO.address))
                    throw new InvalidFieldException("Invalid address!!!");

                if (!await _validationService.CheckText(userDTO.bankName))
                    throw new InvalidFieldException("Invalid bankName!!!");

                if (!await _validationService.CheckText(userDTO.bankAccount))
                    throw new InvalidFieldException("Invalid bankAccount!!!");

                if (userDTO.status < 0 || userDTO.status > 2)
                    throw new InvalidFieldException("Status must be 0(ACTIVE) or 1(INACTIVE) or 2(BLOCKED)!!!");

                if (userDTO.createBy != null && userDTO.createBy.Length >= 0)
                {
                    if (userDTO.createBy.Equals("string"))
                        userDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(userDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (userDTO.updateBy != null && userDTO.updateBy.Length >= 0)
                {
                    if (userDTO.updateBy.Equals("string"))
                        userDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(userDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                userDTO.isDeleted = false;

                User dto = _mapper.Map<User>(userDTO);
                newId.id = await _userRepository.CreateUser(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create User Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete User Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<UserDTO>> GetAllUsers(int pageIndex, int pageSize)
        {
            try
            {
                List<User> userList = await _userRepository.GetAllUsers(pageIndex, pageSize);
                List<UserDTO> list = _mapper.Map<List<UserDTO>>(userList);
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
                    throw new NotFoundException("No User Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateUser(UserDTO userDTO, Guid userId)
        {
            int result;
            try
            {
                if (userDTO.roleId == null || !await _validationService.CheckUUIDFormat(userDTO.roleId))
                    throw new InvalidFieldException("Invalid roleId!!!");

                if (!userDTO.roleId.Equals(ROLE_BUSINESS_MANAGER_ID)
                    && !userDTO.roleId.Equals(ROLE_PROJECT_OWNER_ID)
                    && !userDTO.roleId.Equals(ROLE_ADMIN_ID)
                    && !userDTO.roleId.Equals(ROLE_INVESTOR_ID))
                    throw new NotFoundException("This RoleId is not existed!!!");

                if (userDTO.roleId.Equals(ROLE_BUSINESS_MANAGER_ID) || userDTO.roleId.Equals(ROLE_PROJECT_OWNER_ID))
                {
                    if (userDTO.businessId == null)
                        throw new InvalidFieldException("BusinessId is required for BUSINESS_MANAGER or PROJECT_OWNER!!!");

                    if (!await _validationService.CheckUUIDFormat(userDTO.businessId))
                        throw new InvalidFieldException("Invalid businessId!!!");

                    if (!await _validationService.CheckExistenceId("Business", Guid.Parse(userDTO.businessId)))
                        throw new NotFoundException("This BusinessId is not existed!!!");
                }
                else
                {
                    userDTO.businessId = null;
                }

                if (userDTO.description != null && (userDTO.description.Equals("string") || userDTO.description.Length == 0))
                    userDTO.description = null;

                if (!await _validationService.CheckText(userDTO.lastName))
                    throw new InvalidFieldException("Invalid lastName!!!");

                if (!await _validationService.CheckText(userDTO.firstName))
                    throw new InvalidFieldException("Invalid firstName!!!");

                if (userDTO.image != null && (userDTO.image.Equals("string") || userDTO.image.Length == 0))
                    userDTO.image = null;

                if (userDTO.phoneNum == null || userDTO.phoneNum.Length == 0 || !await _validationService.CheckPhoneNumber(userDTO.phoneNum))
                    throw new InvalidFieldException("Invalid phoneNum!!!");

                if (!await _validationService.CheckText(userDTO.idCard))
                    throw new InvalidFieldException("Invalid idCard!!!");

                if (userDTO.email == null || userDTO.email.Length == 0 || !await _validationService.CheckEmail(userDTO.email))
                    throw new InvalidFieldException("Invalid email!!!");

                if (!await _validationService.CheckText(userDTO.gender))
                    throw new InvalidFieldException("Invalid gender!!!");

                if (userDTO.dateOfBirth == null || userDTO.dateOfBirth.Length == 0 || !await _validationService.CheckDate(userDTO.dateOfBirth))
                    throw new InvalidFieldException("Invalid dateOfBirth!!!");

                if (!await _validationService.CheckText(userDTO.taxIdentificationNumber))
                    throw new InvalidFieldException("Invalid taxIdentificationNumber!!!");

                if (!await _validationService.CheckText(userDTO.city))
                    throw new InvalidFieldException("Invalid city!!!");

                if (!await _validationService.CheckText(userDTO.district))
                    throw new InvalidFieldException("Invalid district!!!");

                if (!await _validationService.CheckText(userDTO.address))
                    throw new InvalidFieldException("Invalid address!!!");

                if (!await _validationService.CheckText(userDTO.bankName))
                    throw new InvalidFieldException("Invalid bankName!!!");

                if (!await _validationService.CheckText(userDTO.bankAccount))
                    throw new InvalidFieldException("Invalid bankAccount!!!");

                if (userDTO.status < 0 || userDTO.status > 2)
                    throw new InvalidFieldException("Status must be 0(ACTIVE) or 1(INACTIVE) or 2(BLOCKED)!!!");

                if (userDTO.createBy != null && userDTO.createBy.Length >= 0)
                {
                    if (userDTO.createBy.Equals("string"))
                        userDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(userDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (userDTO.updateBy != null && userDTO.updateBy.Length >= 0)
                {
                    if (userDTO.updateBy.Equals("string"))
                        userDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(userDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                User dto = _mapper.Map<User>(userDTO);
                result = await _userRepository.UpdateUser(dto, userId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update User Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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

/*        public async Task<bool> VerifyOTP(String OTP, String email)
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
        }*/

    }
}

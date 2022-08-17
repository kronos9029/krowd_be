using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Common;
using RevenueSharingInvest.Business.Services.Common.Firebase;
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
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly IInvestorRepository _investorRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IFieldRepository _fieldRepository;

        private readonly IValidationService _validationService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        public UserService(IOptions<AppSettings> appSettings, 
            IUserRepository userRepository, 
            IInvestorRepository investorRepository,
            IBusinessRepository businessRepository,
            IRoleRepository roleRepository,
            IFieldRepository fieldRepository,
            IValidationService validationService,
            IFileUploadService fileUploadService,
            IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _investorRepository = investorRepository;
            _businessRepository = businessRepository;
            _roleRepository = roleRepository;
            _fieldRepository = fieldRepository;
            _validationService = validationService;
            _fileUploadService = fileUploadService;
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
        public async Task<IdDTO> CreateUser(CreateUserDTO userDTO, string? businessId)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (userDTO.roleId == null || !await _validationService.CheckUUIDFormat(userDTO.roleId))
                    throw new InvalidFieldException("Invalid roleId!!!");

                if (!userDTO.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN"))
                    && !userDTO.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER"))
                    && !userDTO.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"))
                    && !userDTO.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                    throw new NotFoundException("This RoleId is not existed!!!");       

                if (!await _validationService.CheckText(userDTO.lastName))
                    throw new InvalidFieldException("Invalid lastName!!!");

                if (!await _validationService.CheckText(userDTO.firstName))
                    throw new InvalidFieldException("Invalid firstName!!!");

                if (userDTO.image != null && (userDTO.image.Equals("string") || userDTO.image.Length == 0))
                    userDTO.image = null;

                if (userDTO.email == null || userDTO.email.Length == 0 || !await _validationService.CheckEmail(userDTO.email))
                    throw new InvalidFieldException("Invalid email!!!");

                //if (userDTO.createBy != null && userDTO.createBy.Length >= 0)
                //{
                //    if (userDTO.createBy.Equals("string"))
                //        userDTO.createBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(userDTO.createBy))
                //        throw new InvalidFieldException("Invalid createBy!!!");
                //}

                //if (userDTO.updateBy != null && userDTO.updateBy.Length >= 0)
                //{
                //    if (userDTO.updateBy.Equals("string"))
                //        userDTO.updateBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(userDTO.updateBy))
                //        throw new InvalidFieldException("Invalid updateBy!!!");
                //}

                //userDTO.isDeleted = false;

                User entity = _mapper.Map<User>(userDTO);

                if (entity.RoleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    entity.Status = Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1);
                }
                else
                {
                    entity.Status = Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0);
                    entity.BusinessId = Guid.Parse(businessId);
                }

                newId.id = await _userRepository.CreateUser(entity);
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
        public async Task<AllUserDTO> GetAllUsers(int pageIndex, int pageSize, string businessId, string role, string status, ThisUserObj currentUser)
        {
            AllUserDTO result = new AllUserDTO();
            result.listOfUser = new List<GetUserDTO>();
            List<User> listEntity = new List<User>();

            bool statusCheck = false;
            string statusErrorMessage = "";
            bool roleCheck = false;
            string roleErrorMessage = "";

            try
            {
                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    int[] statusNum = { 0, 1, 2 };
                    int[] roleNum = { 0, 1, 2, 3 };

                    if (businessId != null)
                    {
                        if (!await _validationService.CheckUUIDFormat(businessId))
                            throw new InvalidFieldException("Invalid businessId!!!");

                        if (!await _validationService.CheckExistenceId("Business", Guid.Parse(businessId)))
                            throw new NotFoundException("This businessId is not existed!!!");
                    }
                    if (role != null)
                    {
                        foreach (int item in roleNum)
                        {
                            if (role.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(item)))
                                roleCheck = true;
                            roleErrorMessage = roleErrorMessage + " '" + Enum.GetNames(typeof(RoleEnum)).ElementAt(item) + "' or";
                        }
                        roleErrorMessage = roleErrorMessage.Remove(roleErrorMessage.Length - 3);
                        if (!roleCheck)
                            throw new InvalidFieldException("ADMIN can view Users with role" + roleErrorMessage + " !!!");

                        if (role.Equals("ADMIN") && businessId != null)
                            throw new InvalidFieldException("businessId is not required for ADMIN role!!!");

                        role = RoleDictionary.role.GetValueOrDefault(role);
                    }
                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("ADMIN can view Users with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfUser = await _userRepository.CountUser(businessId, null, role, status, currentUser.roleId);
                    listEntity = await _userRepository.GetAllUsers(pageIndex, pageSize, businessId, null, role, status, currentUser.roleId);
                }

                else if(currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    int[] statusNum = { 0, 1, 2 };
                    int[] roleNum = { 2, 3 };

                    if (businessId != null && !businessId.Equals(currentUser.businessId))
                        throw new InvalidFieldException("businessId is not match with this BUSINESS_MANAGER's businessId!!!");
                    businessId = currentUser.businessId;

                    if (role != null)
                    {
                        foreach (int item in roleNum)
                        {
                            if (role.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(item)))
                                roleCheck = true;
                            roleErrorMessage = roleErrorMessage + " '" + Enum.GetNames(typeof(RoleEnum)).ElementAt(item) + "' or";
                        }
                        roleErrorMessage = roleErrorMessage.Remove(roleErrorMessage.Length - 3);
                        if (!roleCheck)
                            throw new InvalidFieldException("BUSINESS_MANAGER can view Users with role" + roleErrorMessage + " !!!");

                        role = RoleDictionary.role.GetValueOrDefault(role);
                    }
                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("BUSINESS_MANAGER can view Users with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfUser = await _userRepository.CountUser(businessId, null, role, status, currentUser.roleId);
                    listEntity = await _userRepository.GetAllUsers(pageIndex, pageSize, businessId, null, role, status, currentUser.roleId);
                }

                else if(currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    int[] statusNum = { 0, 1, 2 };
                    int[] roleNum = { 3 };

                    if (role != null)
                    {
                        foreach (int item in roleNum)
                        {
                            if (role.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(item)))
                                roleCheck = true;
                            roleErrorMessage = roleErrorMessage + " '" + Enum.GetNames(typeof(RoleEnum)).ElementAt(item) + "' or";
                        }
                        roleErrorMessage = roleErrorMessage.Remove(roleErrorMessage.Length - 3);
                        if (!roleCheck)
                            throw new InvalidFieldException("PROJECT_MANAGER can view Users with role" + roleErrorMessage + " !!!");

                        role = RoleDictionary.role.GetValueOrDefault(role);
                    }
                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("PROJECT_MANAGER can view Users with status" + statusErrorMessage + " !!!");
                    }

                    result.numOfUser = await _userRepository.CountUser(null, currentUser.userId, role, status, currentUser.roleId);
                    listEntity = await _userRepository.GetAllUsers(pageIndex, pageSize, null, currentUser.userId, role, status, currentUser.roleId);
                }               
                
                List<GetUserDTO> listDTO = _mapper.Map<List<GetUserDTO>>(listEntity);

                foreach (GetUserDTO item in listDTO)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    item.business = _mapper.Map<GetBusinessDTO>(await _businessRepository.GetBusinessByUserId(Guid.Parse(item.id)));
                    if (item.business != null)
                    {
                        item.business.manager = _mapper.Map<BusinessManagerUserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(item.business.id)));
                        item.business.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(item.business.id)));
                    }                    

                    item.role = _mapper.Map<RoleDTO>(await _roleRepository.GetRoleByUserId(Guid.Parse(item.id)));

                    result.listOfUser.Add(item);
                }               
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetUserDTO> GetUserById(Guid userId)
        {
            try
            {
                User user = await _userRepository.GetUserById(userId);
                GetUserDTO userDTO = _mapper.Map<GetUserDTO>(user);
                if (userDTO == null)
                    throw new NotFoundException("No User Object Found!");

                userDTO.createDate = await _validationService.FormatDateOutput(userDTO.createDate);
                userDTO.updateDate = await _validationService.FormatDateOutput(userDTO.updateDate);

                userDTO.business = _mapper.Map<GetBusinessDTO>(await _businessRepository.GetBusinessByUserId(Guid.Parse(userDTO.id)));
                userDTO.role = _mapper.Map<RoleDTO>(await _roleRepository.GetRoleByUserId(Guid.Parse(userDTO.id)));

                return userDTO;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        } 
        

        public async Task<GetUserDTO> BusinessManagerGetUserById(String businesId, Guid userId)
        {
            try
            {
                User user = await _userRepository.BusinessManagerGetUserById(Guid.Parse(businesId), userId);

                GetUserDTO userDTO = _mapper.Map<GetUserDTO>(user);
                if (userDTO == null)
                    throw new NotFoundException("No User Object Found!");

                userDTO.createDate = await _validationService.FormatDateOutput(userDTO.createDate);
                userDTO.updateDate = await _validationService.FormatDateOutput(userDTO.updateDate);

                userDTO.business = _mapper.Map<GetBusinessDTO>(await _businessRepository.GetBusinessByUserId(Guid.Parse(userDTO.id)));
                userDTO.role = _mapper.Map<RoleDTO>(await _roleRepository.GetRoleByUserId(Guid.Parse(userDTO.id)));

                return userDTO;

            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<GetUserDTO> ProjectManagerGetUserbyId(string managerId, Guid userId)
        {
            try
            {
                User user = await _userRepository.ProjectManagerGetUserbyId(Guid.Parse(managerId), userId);

                GetUserDTO userDTO = _mapper.Map<GetUserDTO>(user);
                if (userDTO == null)
                    throw new NotFoundException("No User Object Found!");

                userDTO.createDate = await _validationService.FormatDateOutput(userDTO.createDate);
                userDTO.updateDate = await _validationService.FormatDateOutput(userDTO.updateDate);

                userDTO.business = _mapper.Map<GetBusinessDTO>(await _businessRepository.GetBusinessByUserId(Guid.Parse(userDTO.id)));
                userDTO.role = _mapper.Map<RoleDTO>(await _roleRepository.GetRoleByUserId(Guid.Parse(userDTO.id)));

                return userDTO;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public async Task<GetUserDTO> GetUserByEmail(String email)
        {
            try
            {
                User user = await _userRepository.GetUserByEmail(email);
                GetUserDTO userDTO = _mapper.Map<GetUserDTO>(user);
                if(userDTO != null)
                {
                    userDTO.createDate = await _validationService.FormatDateOutput(userDTO.createDate);
                    userDTO.updateDate = await _validationService.FormatDateOutput(userDTO.updateDate);

                    userDTO.business = _mapper.Map<GetBusinessDTO>(await _businessRepository.GetBusinessByUserId(Guid.Parse(userDTO.id)));
                    userDTO.role = _mapper.Map<RoleDTO>(await _roleRepository.GetRoleByUserId(Guid.Parse(userDTO.id)));
                }
                return userDTO;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<User>> GetUserByBusinessId(Guid businessId)
        {
            try
            {
                List<User> listEntity = await _userRepository.GetUserByBusinessId(businessId);

                return listEntity;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateUser(UpdateUserDTO userDTO, Guid userId)
        {
            int result;
            try
            {
                if (userDTO.roleId == null || !await _validationService.CheckUUIDFormat(userDTO.roleId))
                    throw new InvalidFieldException("Invalid roleId!!!");

                if (!userDTO.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN"))
                    && !userDTO.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER"))
                    && !userDTO.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"))
                    && !userDTO.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                    throw new NotFoundException("This RoleId is not existed!!!");

                if (userDTO.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")) || userDTO.roleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
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

                if (userDTO.dateOfBirth == null || userDTO.dateOfBirth.Length == 0 || !await _validationService.CheckDOB(userDTO.dateOfBirth))
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

                //if (userDTO.status < 0 || userDTO.status > 2)
                //    throw new InvalidFieldException("Status must be 0(ACTIVE) or 1(INACTIVE) or 2(BLOCKED)!!!");

                //if (userDTO.createBy != null && userDTO.createBy.Length >= 0)
                //{
                //    if (userDTO.createBy.Equals("string"))
                //        userDTO.createBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(userDTO.createBy))
                //        throw new InvalidFieldException("Invalid createBy!!!");
                //}

                //if (userDTO.updateBy != null && userDTO.updateBy.Length >= 0)
                //{
                //    if (userDTO.updateBy.Equals("string"))
                //        userDTO.updateBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(userDTO.updateBy))
                //        throw new InvalidFieldException("Invalid updateBy!!!");
                //}

                User entity = _mapper.Map<User>(userDTO);

                if(userDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseUser(userDTO.image, RoleDictionary.role.GetValueOrDefault("ADMIN"));
                }

                result = await _userRepository.UpdateUser(entity, userId);
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

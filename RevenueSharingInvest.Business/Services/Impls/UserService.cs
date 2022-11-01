using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Business.Services.Extensions.Security;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
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
        private readonly IInvestorTypeRepository _investorTypeRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IProjectWalletRepository _projectWalletRepository;
        private readonly IProjectEntityRepository _projectEntityRepository;

        private readonly IValidationService _validationService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        public UserService(IOptions<AppSettings> appSettings, 
            IUserRepository userRepository, 
            IInvestorRepository investorRepository,
            IInvestorTypeRepository investorTypeRepository,
            IBusinessRepository businessRepository,
            IRoleRepository roleRepository,
            IFieldRepository fieldRepository,
            IProjectWalletRepository projectWalletRepository,
            IProjectEntityRepository projectEntityRepository,
            IValidationService validationService,
            IFileUploadService fileUploadService,
            IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _investorRepository = investorRepository;
            _investorTypeRepository = investorTypeRepository;
            _businessRepository = businessRepository;
            _roleRepository = roleRepository;
            _fieldRepository = fieldRepository;
            _projectWalletRepository = projectWalletRepository;
            _projectEntityRepository = projectEntityRepository;

            _validationService = validationService;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }

        //CREATE
        public async Task<IdDTO> CreateUser(CreateUserDTO userDTO, ThisUserObj currentUser)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (userDTO.businessId == null || !await _validationService.CheckUUIDFormat(userDTO.businessId))
                    throw new InvalidFieldException("Invalid businessId " + userDTO.businessId + " !!!");

                if (!await _validationService.CheckExistenceId("Business", Guid.Parse(userDTO.businessId)))
                    throw new NotFoundException("This businessId " + userDTO.businessId + " is not existed!!!");

                if (currentUser.roleId.Equals(currentUser.adminRoleId) && await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(userDTO.businessId)) != null)
                    throw new InvalidFieldException("This Business has a BUSINESS_MANAGER already!!!");

                if (!await _validationService.CheckText(userDTO.lastName))
                    throw new InvalidFieldException("Invalid lastName!!!");

                if (!await _validationService.CheckText(userDTO.firstName))
                    throw new InvalidFieldException("Invalid firstName!!!");

                if (userDTO.image != null && (userDTO.image.Equals("string") || userDTO.image.Length == 0))
                    userDTO.image = null;

                if (userDTO.email == null || userDTO.email.Length == 0 || !await _validationService.CheckEmail(userDTO.email))
                    throw new InvalidFieldException("Invalid email!!!");

                if (await _userRepository.GetUserByEmail(userDTO.email) != null)
                    throw new InvalidFieldException("This email is existed!!!");

                User entity = _mapper.Map<User>(userDTO);

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    Data.Models.Entities.Business business = await _businessRepository.GetBusinessById(Guid.Parse(userDTO.businessId));
                    entity.RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER"));
                    entity.BusinessId = business.Id;
                    entity.Description = "Business Manager of " + business.Name;
                }
                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    Data.Models.Entities.Business business = await _businessRepository.GetBusinessByUserId(Guid.Parse(currentUser.userId));
                    entity.RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"));
                    entity.BusinessId = Guid.Parse(currentUser.businessId);
                    entity.Description = "Project Manager of " + business.Name;
                    entity.SecretKey = GenerateSecurityKey.GenerateSecretKey();
                }
                entity.Status = Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0);
                entity.CreateBy = Guid.Parse(currentUser.userId);

                newId.id = await _userRepository.CreateUser(entity);

                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create User Object!");
                else
                {
                    if (currentUser.roleId.Equals(currentUser.businessManagerRoleId))
                    {
                        //Tạo ví P1, P2, P5 cho PROJECT_MANAGER
                        ProjectWallet projectWallet = new ProjectWallet();
                        projectWallet.ProjectManagerId = Guid.Parse(newId.id);
                        projectWallet.CreateBy = Guid.Parse(currentUser.userId);

                        projectWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P1"));
                        await _projectWalletRepository.CreateProjectWallet(projectWallet);
                        projectWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P2"));
                        await _projectWalletRepository.CreateProjectWallet(projectWallet);
                        projectWallet.WalletTypeId = Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P5"));
                        await _projectWalletRepository.CreateProjectWallet(projectWallet);
                    }
                    else if (currentUser.roleId.Equals(currentUser.adminRoleId))
                    {
                        //Cập nhật Status cho Business từ INACTIVE thành ACTIVE
                        await _businessRepository.UpdateBusinessStatus(Guid.Parse(userDTO.businessId), BusinessStatusEnum.ACTIVE.ToString(), Guid.Parse(currentUser.userId));
                    }
                }
                return newId;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //DELETE
        //public async Task<int> DeleteUserById(Guid userId)
        //{
        //    int result;
        //    try
        //    {
        //        result = await _userRepository.DeleteUserById(userId);
        //        if (result == 0)
        //            throw new DeleteObjectException("Can not delete User Object!");
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        LoggerService.Logger(e.ToString());
        //        throw new Exception(e.Message);
        //    }
        //}

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
                    item.lastName = (item.lastName == null) ? "" : item.lastName; 
                    item.firstName = (item.firstName == null) ? "" : item.firstName; 
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    item.role = _mapper.Map<RoleDTO>(await _roleRepository.GetRoleByUserId(Guid.Parse(item.id)));
                    if (item.role.id.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")) || item.role.id.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                    {
                        item.business = _mapper.Map<GetBusinessDTO>(await _businessRepository.GetBusinessByUserId(Guid.Parse(item.id)));
                        if (item.business != null)
                        {
                            item.business.manager = _mapper.Map<BusinessManagerUserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(item.business.id)));
                            item.business.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(item.business.id)));
                        }
                    }
                    //else if (item.role.id.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                    //{
                    //    item.investor = _mapper.Map<GetInvestorDTO>(await _investorRepository.GetInvestorByUserId(Guid.Parse(item.id)));
                    //    if (item.investor != null)
                    //    {
                    //        item.investor.investorType = _mapper.Map<UserInvestorTypeDTO>(await _investorTypeRepository.GetInvestorTypeByInvestorId(Guid.Parse(item.investor.id)));
                    //    }
                    //}

                    result.listOfUser.Add(item);
                }               
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
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
                    throw new NotFoundException("No User Found!");

                userDTO.lastName = (userDTO.lastName == null) ? "" : userDTO.lastName;
                userDTO.firstName = (userDTO.firstName == null) ? "" : userDTO.firstName;
                userDTO.createDate = await _validationService.FormatDateOutput(userDTO.createDate);
                userDTO.updateDate = await _validationService.FormatDateOutput(userDTO.updateDate);

                userDTO.role = _mapper.Map<RoleDTO>(await _roleRepository.GetRoleByUserId(Guid.Parse(userDTO.id)));
                if (userDTO.role.id.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")) || userDTO.role.id.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                {
                    userDTO.business = _mapper.Map<GetBusinessDTO>(await _businessRepository.GetBusinessByUserId(Guid.Parse(userDTO.id)));
                    if (userDTO.business != null)
                    {
                        userDTO.business.manager = _mapper.Map<BusinessManagerUserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(userDTO.business.id)));
                        userDTO.business.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(userDTO.business.id)));
                    }
                }
                //else if (userDTO.role.id.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                //{
                //    userDTO.investor = _mapper.Map<GetInvestorDTO>(await _investorRepository.GetInvestorByUserId(Guid.Parse(userDTO.id)));
                //    if (userDTO.investor != null)
                //    {
                //        userDTO.investor.investorType = _mapper.Map<UserInvestorTypeDTO>(await _investorTypeRepository.GetInvestorTypeByInvestorId(Guid.Parse(userDTO.investor.id)));
                //    }
                //}

                return userDTO;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        } 

        public async Task<IntegrateInfo> GetIntegrateInfoByEmailAndProjectId(string email, string projectId)
        {
            try
            { 
                IntegrateInfo info = await _userRepository.GetIntegrateInfoByEmailAndProjectId(email, Guid.Parse(projectId));

                return info;
            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
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
                LoggerService.Logger(e.ToString());
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
                LoggerService.Logger(e.ToString());
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
                LoggerService.Logger(e.ToString());
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
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateUser(UpdateUserDTO userDTO, Guid userId, ThisUserObj currentUser)
        {
            int result;
            try
            {
                User user = await _userRepository.GetUserById(userId);
                if (user == null)
                    throw new InvalidFieldException("This userId is not existed!!!");

                if (!userId.ToString().Equals(currentUser.userId))
                    throw new InvalidFieldException("id is not match with this Updater's Id!!!");

                if (userDTO.businessId != null && !userDTO.businessId.Equals(currentUser.businessId))
                    throw new InvalidFieldException("businessId is not match with this Updater's businessId!!!");

                if (userDTO.description != null && (userDTO.description.Equals("string") || userDTO.description.Length == 0))
                    userDTO.description = null;

                if (userDTO.image != null && (userDTO.image.Equals("string") || userDTO.image.Length == 0))
                    userDTO.image = null;

                if (userDTO.phoneNum != null && (userDTO.phoneNum.Length == 0 || !await _validationService.CheckPhoneNumber(userDTO.phoneNum)))
                    throw new InvalidFieldException("Invalid phoneNum!!!");

                if (userDTO.dateOfBirth != null && (userDTO.dateOfBirth.Length == 0 || !await _validationService.CheckDOB(userDTO.dateOfBirth)))
                    throw new InvalidFieldException("Invalid dateOfBirth!!!");

                User entity = _mapper.Map<User>(userDTO);

                entity.RoleId = Guid.Parse(currentUser.roleId);
                entity.UpdateBy = Guid.Parse(currentUser.userId);
                if (userDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseUser(userDTO.image, currentUser.userId);
                }

                result = await _userRepository.UpdateUser(entity, userId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update User Object!");
                else
                {
                    if (currentUser.roleId.Equals(currentUser.projectManagerRoleId) && userDTO.phoneNum != null && (!userDTO.phoneNum.Equals(user.PhoneNum.ToString()) || user.PhoneNum == null))
                    {
                        await _projectEntityRepository.UpdateProjectManagerContactExtension(Guid.Parse(currentUser.userId), userDTO.phoneNum);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        private string GenerateOTP(Guid UserId)
        {
            string[] OTP = UserId.ToString().Split("-");

            return OTP[0];
        }

        //UPDATE STATUS
        public async Task<int> UpdateUserStatus(Guid userId, string status, ThisUserObj currentUser)
        {
            int result;
            bool statusCheck = false;
            string statusErrorMessage = "";
            try
            {
                User user = await _userRepository.GetUserById(userId);

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")) && user.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                    throw new InvalidFieldException("Can not update ADMIN's status!!!");

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    if (!user.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                        throw new InvalidFieldException("BUSINESS_MANAGER can update status of PROJECT_MANAGER only!!!");
                    if (!user.BusinessId.ToString().Equals(currentUser.businessId))
                        throw new InvalidFieldException("The PROJECT_MANAGER with this businessId is not match with this BUSINESS_MANAGER's businessId!!!");
                }

                for (int item = 0; item < Enum.GetNames(typeof(ObjectStatusEnum)).Count(); item++)
                {
                    if (status.Equals(Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item)))
                        statusCheck = true;
                    statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item) + "' or";
                }
                statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                if (!statusCheck)
                    throw new InvalidFieldException("ADMIN can update Users with status" + statusErrorMessage + " !!!");

                result = await _userRepository.UpdateUserStatus(userId, status, Guid.Parse(currentUser.userId));

                if (result == 0)
                    throw new UpdateObjectException("Can not update User status!");
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE EMAIL
        public async Task<int> UpdateUserEmail(Guid userId, string email, ThisUserObj currentUser)
        {
            int result;
            try
            {
                User user = await _userRepository.GetUserById(userId);

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    if (!user.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                        throw new InvalidFieldException("ADMIN can update email of BUSINESS_MANAGER only!!!");
                }

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    if (!user.RoleId.ToString().Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                        throw new InvalidFieldException("BUSINESS_MANAGER can update email of PROJECT_MANAGER only!!!");
                    if (!user.BusinessId.ToString().Equals(currentUser.businessId))
                        throw new InvalidFieldException("The PROJECT_MANAGER with this businessId is not match with this BUSINESS_MANAGER's businessId!!!");
                }

                if (await _userRepository.GetUserByEmail(email) != null)
                    throw new InvalidFieldException("This email is existed!!!");

                result = await _userRepository.UpdateUserEmail(userId, email, Guid.Parse(currentUser.userId));

                if (result == 0)
                    throw new UpdateObjectException("Can not update User Object!");
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<string> GetProjectIdByManagerEmail(string email)
        {
            try
            {
                string projectId = Convert.ToString(await _userRepository.GetProjectIdByManagerEmail(email));
                return projectId;
            }catch(Exception e)
            {
                LoggerService.Logger(e.Message);
                throw new Exception(e.Message);
            }
        }


    }
}

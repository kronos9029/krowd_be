﻿using AutoMapper;
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
        public async Task<IdDTO> CreateUser(CreateUserDTO userDTO, ThisUserObj currentUser)
        {
            IdDTO newId = new IdDTO();
            try
            {   
                if (!await _validationService.CheckText(userDTO.lastName))
                    throw new InvalidFieldException("Invalid lastName!!!");

                if (!await _validationService.CheckText(userDTO.firstName))
                    throw new InvalidFieldException("Invalid firstName!!!");

                if (userDTO.image != null && (userDTO.image.Equals("string") || userDTO.image.Length == 0))
                    userDTO.image = null;

                if (userDTO.email == null || userDTO.email.Length == 0 || !await _validationService.CheckEmail(userDTO.email))
                    throw new InvalidFieldException("Invalid email!!!");

                User entity = _mapper.Map<User>(userDTO);

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    entity.RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER"));                   
                }
                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    entity.RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER"));
                }
                entity.Status = Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(1);
                entity.CreateBy = Guid.Parse(currentUser.userId);

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
        public async Task<int> UpdateUser(UpdateUserDTO userDTO, Guid userId, ThisUserObj currentUser)
        {
            int result;
            try
            {
                if (!userId.Equals(currentUser.userId))
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
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private string GenerateOTP(Guid UserId)
        {
            string[] OTP = UserId.ToString().Split("-");

            return OTP[0];
        }

        public async Task<int> UpdateUserStatus(Guid userId, string status, ThisUserObj currentUser)
        {
            int result;
            bool statusCheck = false;
            string statusErrorMessage = "";
            try
            {
                User user = await _userRepository.GetUserById(userId);

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")) && user.RoleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                    throw new InvalidFieldException("Can not update ADMIN's status!!!");

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    if (!user.RoleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                        throw new InvalidFieldException("BUSINESS_MANAGER can update status of PROJECT_MANAGER only!!!");
                    if (!user.BusinessId.Equals(currentUser.businessId))
                        throw new InvalidFieldException("The PROJECT_MANAGER with this businessId is not match with this BUSINESS_MANAGER's businessId!!!");
                }

                for (int item = 0; item < Enum.GetNames(typeof(ObjectStatusEnum)).Count(); item ++)
                {
                    if (status.Equals(Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item)))
                        statusCheck = true;
                    statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(item) + "' or";
                }
                statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                if (!statusCheck)
                    throw new InvalidFieldException("ADMIN can view Users with status" + statusErrorMessage + " !!!");

                result = await _userRepository.UpdateUserStatus(userId, status, Guid.Parse(currentUser.userId));

                if (result == 0)
                    throw new UpdateObjectException("Can not update User Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> UpdateUserEmail(Guid userId, string email, ThisUserObj currentUser)
        {
            int result;
            try
            {
                User user = await _userRepository.GetUserById(userId);

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    if (!user.RoleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                        throw new InvalidFieldException("ADMIN can update email of BUSINESS_MANAGER only!!!");
                }

                if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("BUSINESS_MANAGER")))
                {
                    if (!user.RoleId.Equals(RoleDictionary.role.GetValueOrDefault("PROJECT_MANAGER")))
                        throw new InvalidFieldException("BUSINESS_MANAGER can update email of PROJECT_MANAGER only!!!");
                    if (!user.BusinessId.Equals(currentUser.businessId))
                        throw new InvalidFieldException("The PROJECT_MANAGER with this businessId is not match with this BUSINESS_MANAGER's businessId!!!");
                }

                if (await _userRepository.GetUserByEmail(email) != null)
                    throw new InvalidFieldException("This email is already used!!!");

                result = await _userRepository.UpdateUserEmail(userId, email, Guid.Parse(currentUser.userId));

                if (result == 0)
                    throw new UpdateObjectException("Can not update User Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

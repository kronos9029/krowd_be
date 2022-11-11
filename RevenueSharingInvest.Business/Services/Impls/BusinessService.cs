using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessFieldRepository _businessFieldRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectWalletRepository _projectWalletRepository;
        private readonly IProjectEntityRepository _projectEntityRepository;

        private readonly IValidationService _validationService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public BusinessService(IBusinessRepository businessRepository, 
            IBusinessFieldRepository businessFieldRepository, 
            IValidationService validationService, 
            IUserRepository userRepository,
            IFieldRepository fieldRepository,
            IProjectRepository projectRepository,
            IProjectWalletRepository projectWalletRepository,
            IProjectEntityRepository projectEntityRepository,
            IFileUploadService fileUploadService,
            IProjectService projectService,
            IMapper mapper)
        {
            _businessRepository = businessRepository;
            _businessFieldRepository = businessFieldRepository;
            _fieldRepository = fieldRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _projectWalletRepository = projectWalletRepository;
            _projectEntityRepository = projectEntityRepository;

            _validationService = validationService;
            _fileUploadService = fileUploadService;
            _projectService = projectService;

            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllBusinessData()
        {
            int result;
            try
            {
                result = await _businessRepository.ClearAllBusinessData();
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateBusiness(CreateBusinessDTO businessDTO, List<string> fieldIdList, ThisUserObj currentUser)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!currentUser.businessId.Equals(""))
                {
                    throw new CreateObjectException("This BUSINESS_MANAGER has a business already!!!");
                }

                //Validate
                //List<string> - List FieldId
                if (fieldIdList.Count() == 0)
                    throw new InvalidFieldException("fieldIdList contain at least 1 fieldId!!!");

                foreach (string fieldId in fieldIdList)
                {
                    if (fieldId == null || !await _validationService.CheckUUIDFormat(fieldId))
                        throw new InvalidFieldException("Invalid fieldId " + fieldId + " !!!");

                    if (!await _validationService.CheckExistenceId("Field", Guid.Parse(fieldId)))
                        throw new NotFoundException("This fieldId " + fieldId + " is not existed!!!");
                }

                //BusinessDTO object
                if (!await _validationService.CheckText(businessDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (businessDTO.phoneNum == null || businessDTO.phoneNum.Length == 0 || !await _validationService.CheckPhoneNumber(businessDTO.phoneNum))
                    throw new InvalidFieldException("Invalid phoneNum!!!");

                if (businessDTO.email == null || businessDTO.email.Length == 0 || !await _validationService.CheckEmail(businessDTO.email))
                    throw new InvalidFieldException("Invalid email!!!");

                if (!await _validationService.CheckText(businessDTO.taxIdentificationNumber))
                    throw new InvalidFieldException("Invalid taxIdentificationNumber!!!");

                Data.Models.Entities.Business entity = _mapper.Map<Data.Models.Entities.Business>(businessDTO);

                entity.Status = BusinessStatusEnum.INACTIVE.ToString();
                entity.CreateBy = Guid.Parse(currentUser.userId);

                newId.id = await _businessRepository.CreateBusiness(entity);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Business Object!");
                else
                {
                    //if (await _userRepository.UpdateBusinessIdForBuM(Guid.Parse(newId.id), Guid.Parse(currentUser.userId)) == 0)
                    //{
                    //    await _businessRepository.DeleteBusinessByBusinessId(Guid.Parse(newId.id));
                    //    throw new UpdateObjectException("Can not update businessId for User(BUSINESS_MANAGER) Object!");
                    //}
                    foreach (string fieldId in fieldIdList)
                    {
                        if (await _businessFieldRepository.CreateBusinessField(Guid.Parse(newId.id), Guid.Parse(fieldId), Guid.Parse(currentUser.userId)) == 0)//ráp authen sửa createBy và updateBy
                        {
                            //Xóa các object BusinessField vừa mới tạo
                            await _businessFieldRepository.DeleteBusinessFieldByBusinessId(Guid.Parse(newId.id));
                            //Xóa object Business vừa mới tạo
                            await _businessRepository.DeleteBusinessByBusinessId(Guid.Parse(newId.id));
                            //Update businessId của BuM lại thành null
                            await _userRepository.UpdateBusinessIdForBuM(null, Guid.Parse(currentUser.userId));
                            throw new CreateObjectException("Can not create BusinessField Object has businessId: " + newId.id + " and fieldId: " + fieldId + " ! Create Business object failed!");
                        }                           
                    }
                    return newId;
                }               
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteBusinessById(Guid businessId, ThisUserObj currentUser)
        {
            int result = 0;
            try
            {
                Data.Models.Entities.Business business = await _businessRepository.GetBusinessById(businessId);

                if (business != null)
                {
                    if (business.Status.Equals(BusinessStatusEnum.INACTIVE.ToString()))
                    {
                        List<Project> projectList = await _projectRepository.GetAllProjects(0, 0, businessId.ToString(), null, null , null, 0, null, null, currentUser.roleId);
                        foreach (Project item in projectList)
                        {
                            await _projectService.DeleteProjectById(item.Id);                           
                        }
                        await _projectWalletRepository.DeleteProjectWalletByBusinessId(businessId);
                        await _userRepository.DeleteUserByBusinessId(businessId);
                        await _businessFieldRepository.DeleteBusinessFieldByBusinessId(businessId);
                        result = await _businessRepository.DeleteBusinessById(businessId);
                    }                       
                }
             
                if (result == 0)
                    throw new DeleteObjectException("Can not delete Business Object. Business can be deleted if status is INACTIVE!!!");
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<AllBusinessDTO> GetAllBusiness(int pageIndex, int pageSize, string status, string name, string orderBy, string order, ThisUserObj currentUser)
        {
            AllBusinessDTO result = new AllBusinessDTO();
            result.listOfBusiness = new List<GetBusinessDTO>();
            List<RevenueSharingInvest.Data.Models.Entities.Business> listEntity = new List<Data.Models.Entities.Business>();

            string orderByErrorMessage = "";
            bool checkOrderError = false;
            string orderErrorMessage = "";
            bool statusCheck = false;
            string statusErrorMessage = "";
            try
            {
                if (currentUser.roleId.Equals(""))
                {
                    int[] statusNum = { 1 };

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("GUEST can view Businesses with status" + statusErrorMessage + " !!!");
                    }
                }

                else if (currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("ADMIN")))
                {
                    int[] statusNum = { 0, 1, 2 };

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("ADMIN can view Businesses with status" + statusErrorMessage + " !!!");
                    }
                }

                else if(currentUser.roleId.Equals(RoleDictionary.role.GetValueOrDefault("INVESTOR")))
                {
                    int[] statusNum = { 1 };

                    if (status != null)
                    {
                        foreach (int item in statusNum)
                        {
                            if (status.Equals(Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(item)))
                                statusCheck = true;
                            statusErrorMessage = statusErrorMessage + " '" + Enum.GetNames(typeof(BusinessStatusEnum)).ElementAt(item) + "' or";
                        }
                        statusErrorMessage = statusErrorMessage.Remove(statusErrorMessage.Length - 3);
                        if (!statusCheck)
                            throw new InvalidFieldException("INVESTOR can view Businesses with status" + statusErrorMessage + " !!!");
                    }                   
                }

                if (orderBy != null)
                {
                    if (!OrderFieldDictionary.business.ContainsKey(orderBy))
                    {
                        foreach (KeyValuePair<string, string> pair in OrderFieldDictionary.business)
                        {
                            orderByErrorMessage = orderByErrorMessage + " " + pair.Key + " or";
                        }
                        orderByErrorMessage = orderByErrorMessage.Remove(orderByErrorMessage.Length - 2);
                        throw new InvalidFieldException("orderBy must be" + orderByErrorMessage + " !!!");
                    }
                    else
                        orderBy = OrderFieldDictionary.business.GetValueOrDefault(orderBy);
                }
                if (order != null)
                {
                    for (int o = 0; o < Enum.GetNames(typeof(OrderEnum)).Length; o++)
                    {
                        if (order.Equals(Enum.GetNames(typeof(OrderEnum)).ElementAt(o)))
                            checkOrderError = true;
                        orderErrorMessage = orderErrorMessage + " " + Enum.GetNames(typeof(OrderEnum)).ElementAt(o) + " or";
                    }
                    orderErrorMessage = orderErrorMessage.Remove(orderErrorMessage.Length - 2);
                    if (!checkOrderError)
                        throw new InvalidFieldException("order must be" + orderErrorMessage + " !!!");
                }

                result.numOfBusiness = await _businessRepository.CountBusiness(status, name, currentUser.roleId);
                listEntity = await _businessRepository.GetAllBusiness(pageIndex, pageSize, status, name, orderBy, order, currentUser.roleId);

                List<GetBusinessDTO> listDTO = _mapper.Map<List<GetBusinessDTO>>(listEntity);

                foreach (GetBusinessDTO item in listDTO)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    item.manager = _mapper.Map<BusinessManagerUserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(item.id)));
                    item.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(item.id)));

                    result.listOfBusiness.Add(item);
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
        public async Task<GetBusinessDTO> GetBusinessById(Guid businessId)
        {
            try
            {
                RevenueSharingInvest.Data.Models.Entities.Business business = await _businessRepository.GetBusinessById(businessId);
                GetBusinessDTO businessDTO = _mapper.Map<GetBusinessDTO>(business);
                if (businessDTO == null)
                    throw new NotFoundException("No Business Object Found!");

                businessDTO.createDate = await _validationService.FormatDateOutput(businessDTO.createDate);
                businessDTO.updateDate = await _validationService.FormatDateOutput(businessDTO.updateDate);

                businessDTO.manager = _mapper.Map<BusinessManagerUserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(businessDTO.id)));
                businessDTO.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(businessDTO.id)));

                return businessDTO;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }        
        
        //GET BY EMAIL
        public async Task<GetBusinessDTO> GetBusinessByEmail(string email)
        {
            try
            {

                RevenueSharingInvest.Data.Models.Entities.Business business = await _businessRepository.GetBusinessByEmail(email);
                GetBusinessDTO businessDTO = _mapper.Map<GetBusinessDTO>(business);
                if (businessDTO == null)
                    throw new NotFoundException("No Business Object Found!");

                businessDTO.createDate = await _validationService.FormatDateOutput(businessDTO.createDate);
                businessDTO.updateDate = await _validationService.FormatDateOutput(businessDTO.updateDate);

                businessDTO.manager = _mapper.Map<BusinessManagerUserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(businessDTO.id)));
                businessDTO.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(businessDTO.id)));

                return businessDTO;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateBusiness(UpdateBusinessDTO businessDTO, Guid businessId, ThisUserObj currentUser)
        {
            int result;
            try
            {
                Data.Models.Entities.Business business = await _businessRepository.GetBusinessById(businessId);
                if (business == null)
                    throw new InvalidFieldException("This businessId is not existed!!!");

                if (currentUser.roleId.Equals(currentUser.businessManagerRoleId) && !businessId.Equals(Guid.Parse(currentUser.businessId)))
                    throw new InvalidFieldException("This is not your Business!!!");

                if (business.Status.Equals(BusinessStatusEnum.BLOCKED))
                {
                    throw new UnauthorizedException("You Can Not Update This Business Because It Is Blocked!!");
                }

                if (businessDTO.phoneNum != null)
                {
                    if (businessDTO.phoneNum.Length == 0 || !await _validationService.CheckPhoneNumber(businessDTO.phoneNum))
                        throw new InvalidFieldException("Invalid phoneNum!!!");
                }
                
                if (businessDTO.image != null)
                {
                    if (businessDTO.image.Equals("string") || businessDTO.image.Length == 0)
                        businessDTO.image = null;
                }
                
                if (businessDTO.email != null)
                {
                    if (businessDTO.email.Length == 0 || !await _validationService.CheckEmail(businessDTO.email))
                        throw new InvalidFieldException("Invalid email!!!");
                }
                
                if (businessDTO.description != null)
                {
                    if (businessDTO.description.Equals("string") || businessDTO.description.Length == 0)
                        businessDTO.description = null;
                }
                
                if (businessDTO.taxIdentificationNumber != null)
                {
                    if (!await _validationService.CheckText(businessDTO.taxIdentificationNumber))
                        throw new InvalidFieldException("Invalid taxIdentificationNumber!!!");
                }
                
                if (businessDTO.address != null)
                {
                    if (!await _validationService.CheckText(businessDTO.address))
                        throw new InvalidFieldException("Invalid address!!!");
                }             

                RevenueSharingInvest.Data.Models.Entities.Business entity = _mapper.Map<RevenueSharingInvest.Data.Models.Entities.Business>(businessDTO);

                if (businessDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseBusiness(businessDTO.image, businessId.ToString());
                }
                entity.UpdateBy = Guid.Parse(currentUser.userId);

                result = await _businessRepository.UpdateBusiness(entity, businessId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Business Object!");
                else
                {
                    if (businessDTO.email != null && !businessDTO.email.Equals(business.Email.ToString()))
                    {
                        await _projectEntityRepository.UpdateBusinessEmailExtension(businessId, businessDTO.email);
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

        //UPDATE STATUS
        public async Task<int> UpdateBusinessStatus(Guid businessId, string status, ThisUserObj currentUser)
        {
            int result;
            try
            {
                Data.Models.Entities.Business business = await _businessRepository.GetBusinessById(businessId);
                if (business == null)
                    throw new InvalidFieldException("There are no Business has this Id!!!");

                if (currentUser.roleId.Equals(currentUser.adminRoleId))
                {
                    if (!status.Equals(BusinessStatusEnum.INACTIVE.ToString()) && !status.Equals(BusinessStatusEnum.ACTIVE.ToString()) && !status.Equals(BusinessStatusEnum.BLOCKED.ToString()))
                        throw new InvalidFieldException("Status must be INACTIVE or ACTIVE or BLOCKED!!!");
                }

                result = await _businessRepository.UpdateBusinessStatus(businessId, status, Guid.Parse(currentUser.userId));
                if (result == 0)
                    throw new UpdateObjectException("Can not update Business status!");
                return result;
            } 
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY PROJECT ID
        public async Task<Data.Models.Entities.Business> GetBusinessByProjectId(Guid projectId)
        {
            try
            {
                Data.Models.Entities.Business business = await _businessRepository.GetBusinessByProjectId(projectId);

                return business;

            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BUSINESS NAME BY ID
        public async Task<string> GetBusinessNameById(string businessId)
        {
            try
            {
                string businessName = await _businessRepository.GetBusinessNameById(Guid.Parse(businessId));

                return businessName;

            }catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }


    }
}

using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
using RevenueSharingInvest.Business.Services.Common.Firebase;
using RevenueSharingInvest.Data.Models.Constants;
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

        private readonly IValidationService _validationService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        public BusinessService(IBusinessRepository businessRepository, 
            IBusinessFieldRepository businessFieldRepository, 
            IValidationService validationService, 
            IUserRepository userRepository,
            IFieldRepository fieldRepository,
            IFileUploadService fileUploadService,
            IMapper mapper)
        {
            _businessRepository = businessRepository;
            _businessFieldRepository = businessFieldRepository;
            _fieldRepository = fieldRepository;
            _userRepository = userRepository;
            _validationService = validationService;
            _fileUploadService = fileUploadService;
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
                throw new Exception(e.Message);
            }
        }

        ////ADMIN CREATE
        //public async Task<int> AdminCreateBusiness(Data.Models.Entities.Business newBusiness, string email)
        //{
        //    User userObject = await _userRepository.GetUserByEmail(email);
        //    if(userObject == null)
        //    {
        //        throw new NotFoundException("User Not Found!!");
        //    }
        //    else
        //    {
        //        if (!userObject.RoleId.ToString().Equals(ROLE_ADMIN_ID))
        //        {
        //            throw new Exceptions.UnauthorizedAccessException("Only Admin Can Create Business!!");
        //        }
        //        else
        //        {
        //            if(await _businessRepository.CreateBusiness(newBusiness) < 1)
        //            {
        //                throw new CreateBusinessException("Create Business Fail!!");
        //            }
        //        }
        //    }

        //    return 1;
        //}

        //CREATE
        public async Task<IdDTO> CreateBusiness(CreateUpdateBusinessDTO businessDTO, List<string> fieldIdList, string roleId)
        {
            IdDTO newId = new IdDTO();
            try
            {
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

                if (businessDTO.image != null && (businessDTO.image.Equals("string") || businessDTO.image.Length == 0))
                    businessDTO.image = null;

                if (businessDTO.email == null || businessDTO.email.Length == 0 || !await _validationService.CheckEmail(businessDTO.email))
                    throw new InvalidFieldException("Invalid email!!!");

                if (businessDTO.description != null && (businessDTO.description.Equals("string") || businessDTO.description.Length == 0))
                    businessDTO.description = null;

                if (!await _validationService.CheckText(businessDTO.taxIdentificationNumber))
                    throw new InvalidFieldException("Invalid taxIdentificationNumber!!!");

                if (!await _validationService.CheckText(businessDTO.address))
                    throw new InvalidFieldException("Invalid address!!!");

                //if (businessDTO.createBy != null && businessDTO.createBy.Length >= 0)
                //{
                //    if (businessDTO.createBy.Equals("string"))
                //        businessDTO.createBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(businessDTO.createBy))
                //        throw new InvalidFieldException("Invalid createBy!!!");
                //}

                //if (businessDTO.updateBy != null && businessDTO.updateBy.Length >= 0)
                //{
                //    if (businessDTO.updateBy.Equals("string"))
                //        businessDTO.updateBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(businessDTO.updateBy))
                //        throw new InvalidFieldException("Invalid updateBy!!!");
                //}

                //businessDTO.isDeleted = false;
                //
                RevenueSharingInvest.Data.Models.Entities.Business entity = _mapper.Map<RevenueSharingInvest.Data.Models.Entities.Business>(businessDTO);

                entity.Status = Enum.GetNames(typeof(ObjectStatusEnum)).ElementAt(0);

                if (businessDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseBusiness(businessDTO.image, roleId);//sửa role admin sau
                }

                newId.id = await _businessRepository.CreateBusiness(entity);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Business Object!");
                else
                {
                    foreach (string fieldId in fieldIdList)
                    {
                        if (await _businessFieldRepository.CreateBusinessField(Guid.Parse(newId.id), Guid.Parse(fieldId), Guid.Parse(newId.id)) == 0)//ráp authen sửa createBy và updateBy
                        {
                            //Xóa các object BusinessField vừa mới tạo
                            _businessFieldRepository.DeleteBusinessFieldByBusinessId(Guid.Parse(newId.id));
                            //Xóa object Business vừa mới tạo
                            _businessRepository.DeleteBusinessByBusinessId(Guid.Parse(newId.id));
                            throw new CreateObjectException("Can not create BusinessField Object has businessId: " + newId.id + " and fieldId: " + fieldId + " ! Create Business object failed!");
                        }                           
                    }
                    return newId;
                }               
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteBusinessById(Guid businessId)
        {
            int result;
            try
            {
                Data.Models.Entities.Business business = await _businessRepository.GetBusinessById(businessId);

                if (business.Status.Equals(ObjectStatusEnum.INACTIVE.ToString()))
                {
                    result = await _businessRepository.DeleteBusinessById(businessId);
                } else
                {
                    result = 0;
                }

                
                if (result == 0)
                    throw new DeleteObjectException("Can not delete Business Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<AllBusinessDTO> GetAllBusiness(int pageIndex, int pageSize, string? orderBy, string? order, string temp_field_role)
        {
            string orderByErrorMessage = "";
            bool checkOrderError = false;
            string orderErrorMessage = "";
            try
            {
                if (!temp_field_role.Equals("ADMIN") && !temp_field_role.Equals("INVESTOR"))
                    throw new InvalidFieldException("ADMIN or INVESTOR!");

                //if (orderBy != null && (orderBy < 0 || orderBy > Enum.GetNames(typeof(BusinessOrderFieldEnum)).Length - 1))
                //{
                //    for (int field = 0; field < Enum.GetNames(typeof(BusinessOrderFieldEnum)).Length; field++)
                //    {
                //        orderByErrorMessage = orderByErrorMessage + " " + field +":" + Enum.GetNames(typeof(BusinessOrderFieldEnum)).ElementAt(field) + " or";
                //    }
                //    orderByErrorMessage = orderByErrorMessage.Remove(orderByErrorMessage.Length - 2);
                //    throw new InvalidFieldException("orderBy must be" + orderByErrorMessage + " !!!");
                //}

                if (orderBy != null)
                {
                    //for (int item = 0; item < Enum.GetNames(typeof(OrderEnum)).Length; item++)
                    //{
                    //    if (orderBy.Equals(field))
                    //    {
                    //        checkOrderByError = true;
                    //        orderBy = field;
                    //    }                         
                    //    orderByErrorMessage = orderByErrorMessage + " " + field + " or";
                    //}
                    if (!BusinessOrderFieldDictionary.column.ContainsKey(orderBy))
                    {
                        foreach (KeyValuePair<string, string> pair in BusinessOrderFieldDictionary.column)
                        {
                            orderByErrorMessage = orderByErrorMessage + " " + pair.Key + " or";
                        }
                        orderByErrorMessage = orderByErrorMessage.Remove(orderByErrorMessage.Length - 2);
                        throw new InvalidFieldException("orderBy must be" + orderByErrorMessage + " !!!");
                    }
                    else
                        orderBy = BusinessOrderFieldDictionary.column.GetValueOrDefault(orderBy);
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

                AllBusinessDTO result = new AllBusinessDTO();
                result.listOfBusiness = new List<GetBusinessDTO>();

                result.numOfBusiness = await _businessRepository.CountBusiness(temp_field_role);

                List<RevenueSharingInvest.Data.Models.Entities.Business> listEntity = await _businessRepository.GetAllBusiness(pageIndex, pageSize, orderBy, order, temp_field_role);
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
                throw new Exception(e.Message);
            }
        }        
        
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
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateBusiness(CreateUpdateBusinessDTO businessDTO, Guid businessId)
        {
            int result;
            try
            {
                Data.Models.Entities.Business business = await _businessRepository.GetBusinessById(businessId);

                if (business.Status.Equals(ObjectStatusEnum.BLOCKED))
                {
                    throw new UnauthorizedException("You Can Not Update This Business Because It Is Blocked!!");
                }

                if (!await _validationService.CheckText(businessDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (businessDTO.phoneNum == null || businessDTO.phoneNum.Length == 0 || !await _validationService.CheckPhoneNumber(businessDTO.phoneNum))
                    throw new InvalidFieldException("Invalid phoneNum!!!");

                if (businessDTO.image != null && (businessDTO.image.Equals("string") || businessDTO.image.Length == 0))
                    businessDTO.image = null;

                if (businessDTO.email == null || businessDTO.email.Length == 0 || !await _validationService.CheckEmail(businessDTO.email))
                    throw new InvalidFieldException("Invalid email!!!");

                if (businessDTO.description != null && (businessDTO.description.Equals("string") || businessDTO.description.Length == 0))
                    businessDTO.description = null;

                if (!await _validationService.CheckText(businessDTO.taxIdentificationNumber))
                    throw new InvalidFieldException("Invalid taxIdentificationNumber!!!");

                if (!await _validationService.CheckText(businessDTO.address))
                    throw new InvalidFieldException("Invalid address!!!");

                //if (businessDTO.status < 0 || businessDTO.status > 2)
                //    throw new InvalidFieldException("Status must be 0(ACTIVE) or 1(INACTIVE) or 2(BLOCKED)!!!");

                //if (businessDTO.createBy != null && businessDTO.createBy.Length >= 0)
                //{
                //    if (businessDTO.createBy.Equals("string"))
                //        businessDTO.createBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(businessDTO.createBy))
                //        throw new InvalidFieldException("Invalid createBy!!!");
                //}

                //if (businessDTO.updateBy != null && businessDTO.updateBy.Length >= 0)
                //{
                //    if (businessDTO.updateBy.Equals("string"))
                //        businessDTO.updateBy = null;
                //    else if (!await _validationService.CheckUUIDFormat(businessDTO.updateBy))
                //        throw new InvalidFieldException("Invalid updateBy!!!");
                //}

                RevenueSharingInvest.Data.Models.Entities.Business entity = _mapper.Map<RevenueSharingInvest.Data.Models.Entities.Business>(businessDTO);

                if (businessDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseBusiness(businessDTO.image, businessId.ToString());
                }

                result = await _businessRepository.UpdateBusiness(entity, businessId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Business Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> UpdateBusinessStatus(Guid businessId, string status)
        {
            try
            {


                int result = await _businessRepository.UpdateBusinessStatus(businessId, status);
                return result;
            } catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

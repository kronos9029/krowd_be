using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
using RevenueSharingInvest.Business.Services.Common.Firebase;
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
        private readonly String ROLE_ADMIN_ID = "ff54acc6-c4e9-4b73-a158-fd640b4b6940";

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
        public async Task<IdDTO> CreateBusiness(BusinessDTO businessDTO, List<string> fieldIdList)
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

                if (businessDTO.status < 0 || businessDTO.status > 2)
                    throw new InvalidFieldException("Status must be 0(ACTIVE) or 1(INACTIVE) or 2(BLOCKED)!!!");

                if (businessDTO.createBy != null && businessDTO.createBy.Length >= 0)
                {
                    if (businessDTO.createBy.Equals("string"))
                        businessDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(businessDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (businessDTO.updateBy != null && businessDTO.updateBy.Length >= 0)
                {
                    if (businessDTO.updateBy.Equals("string"))
                        businessDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(businessDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                businessDTO.isDeleted = false;
                //
                RevenueSharingInvest.Data.Models.Entities.Business entity = _mapper.Map<RevenueSharingInvest.Data.Models.Entities.Business>(businessDTO);

                if (businessDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseBusiness(businessDTO.image, businessDTO.createBy);
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
                result = await _businessRepository.DeleteBusinessById(businessId);
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
        public async Task<AllBusinessDTO> GetAllBusiness(int pageIndex, int pageSize, string temp_field_role)
        {
            try
            {
                if (!temp_field_role.Equals("ADMIN") && !temp_field_role.Equals("INVESTOR"))
                    throw new InvalidFieldException("ADMIN or INVESTOR!");

                AllBusinessDTO result = new AllBusinessDTO();
                result.listOfBusiness = new List<BusinessDetailDTO>();
                BusinessDetailDTO resultItem = new BusinessDetailDTO();
                resultItem.fieldList = new List<FieldDTO>();

                result.numOfBusiness = await _businessRepository.CountBusiness(temp_field_role);

                List<RevenueSharingInvest.Data.Models.Entities.Business> listEntity = await _businessRepository.GetAllBusiness(pageIndex, pageSize, temp_field_role);
                List<BusinessDTO> listDTO = _mapper.Map<List<BusinessDTO>>(listEntity);

                foreach (BusinessDTO item in listDTO)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    resultItem = _mapper.Map<BusinessDetailDTO>(item);
                    resultItem.manager = _mapper.Map<UserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(item.id)));
                    resultItem.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(item.id)));
                    result.listOfBusiness.Add(resultItem);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<BusinessDetailDTO> GetBusinessById(Guid businessId)
        {
            BusinessDetailDTO result;
            try
            {

                RevenueSharingInvest.Data.Models.Entities.Business business = await _businessRepository.GetBusinessById(businessId);
                BusinessDTO businessDTO = _mapper.Map<BusinessDTO>(business);
                if (businessDTO == null)
                    throw new NotFoundException("No Business Object Found!");

                businessDTO.createDate = await _validationService.FormatDateOutput(businessDTO.createDate);
                businessDTO.updateDate = await _validationService.FormatDateOutput(businessDTO.updateDate);

                result = _mapper.Map<BusinessDetailDTO>(businessDTO);
                result.manager = _mapper.Map<UserDTO>(await _userRepository.GetBusinessManagerByBusinessId(Guid.Parse(businessDTO.id)));
                result.fieldList = _mapper.Map<List<FieldDTO>>(await _fieldRepository.GetCompanyFields(Guid.Parse(businessDTO.id)));

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateBusiness(BusinessDTO businessDTO, Guid businessId)
        {
            int result;
            try
            {
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

                if (businessDTO.status < 0 || businessDTO.status > 2)
                    throw new InvalidFieldException("Status must be 0(ACTIVE) or 1(INACTIVE) or 2(BLOCKED)!!!");

                if (businessDTO.createBy != null && businessDTO.createBy.Length >= 0)
                {
                    if (businessDTO.createBy.Equals("string"))
                        businessDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(businessDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (businessDTO.updateBy != null && businessDTO.updateBy.Length >= 0)
                {
                    if (businessDTO.updateBy.Equals("string"))
                        businessDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(businessDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                RevenueSharingInvest.Data.Models.Entities.Business entity = _mapper.Map<RevenueSharingInvest.Data.Models.Entities.Business>(businessDTO);

                if (businessDTO.image != null)
                {
                    entity.Image = await _fileUploadService.UploadImageToFirebaseBusiness(businessDTO.image, businessDTO.updateBy);
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
    }
}

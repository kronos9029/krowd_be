using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
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
        private readonly IUserRepository _userRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly String ROLE_ADMIN_ID = "ff54acc6-c4e9-4b73-a158-fd640b4b6940";

        public BusinessService(IBusinessRepository businessRepository, IBusinessFieldRepository businessFieldRepository, IValidationService validationService, IUserRepository userRepository, IMapper mapper)
        {
            _businessRepository = businessRepository;
            _businessFieldRepository = businessFieldRepository;
            _userRepository = userRepository;
            _validationService = validationService;
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

                RevenueSharingInvest.Data.Models.Entities.Business dto = _mapper.Map<RevenueSharingInvest.Data.Models.Entities.Business>(businessDTO);
                newId.id = await _businessRepository.CreateBusiness(dto);
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
        public async Task<List<BusinessDTO>> GetAllBusiness(int pageIndex, int pageSize)
        {
            try
            {
                List<RevenueSharingInvest.Data.Models.Entities.Business> businessList = await _businessRepository.GetAllBusiness(pageIndex, pageSize);
                List<BusinessDTO> list = _mapper.Map<List<BusinessDTO>>(businessList);

                foreach (BusinessDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);
                }

                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<BusinessDTO> GetBusinessById(Guid businessId)
        {
            BusinessDTO result;
            try
            {

                RevenueSharingInvest.Data.Models.Entities.Business dto = await _businessRepository.GetBusinessById(businessId);
                result = _mapper.Map<BusinessDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No Business Object Found!");

                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

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

                RevenueSharingInvest.Data.Models.Entities.Business dto = _mapper.Map<RevenueSharingInvest.Data.Models.Entities.Business>(businessDTO);
                result = await _businessRepository.UpdateBusiness(dto, businessId);
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

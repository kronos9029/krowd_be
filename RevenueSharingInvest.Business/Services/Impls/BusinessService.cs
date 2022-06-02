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
        private readonly IUserRepository _userRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly String ROLE_ADMIN_ID = "ff54acc6-c4e9-4b73-a158-fd640b4b6940";

        public BusinessService(IBusinessRepository businessRepository, IValidationService validationService, IUserRepository userRepository, IMapper mapper)
        {
            _businessRepository = businessRepository;
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
        public async Task<IdDTO> CreateBusiness(BusinessDTO businessDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (businessDTO.name == null || !await _validationService.CheckText(businessDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (businessDTO.phoneNum == null || businessDTO.phoneNum.Length == 0 || !await _validationService.CheckPhoneNumber(businessDTO.phoneNum))
                    throw new InvalidFieldException("Invalid phoneNum!!!");

                if (businessDTO.image != null && (businessDTO.image.Equals("string") || businessDTO.image.Length == 0))
                    businessDTO.image = null;

                if (businessDTO.email == null || businessDTO.email.Length == 0 || !await _validationService.CheckEmail(businessDTO.email))
                    throw new InvalidFieldException("Invalid email!!!");

                if (businessDTO.description != null && (businessDTO.description.Equals("string") || businessDTO.description.Length == 0))
                    businessDTO.description = null;

                if (businessDTO.taxIdentificationNumber == null || !await _validationService.CheckText(businessDTO.taxIdentificationNumber))
                    throw new InvalidFieldException("Invalid taxIdentificationNumber!!!");

                if (businessDTO.address == null || !await _validationService.CheckText(businessDTO.address))
                    throw new InvalidFieldException("Invalid address!!!");

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

                RevenueSharingInvest.Data.Models.Entities.Business dto = _mapper.Map<RevenueSharingInvest.Data.Models.Entities.Business>(businessDTO);
                newId.id = await _businessRepository.CreateBusiness(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Business Object!");
                return newId;
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
                if (businessDTO.name == null || !await _validationService.CheckText(businessDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (businessDTO.phoneNum == null || businessDTO.phoneNum.Length == 0 || !await _validationService.CheckPhoneNumber(businessDTO.phoneNum))
                    throw new InvalidFieldException("Invalid phoneNum!!!");

                if (businessDTO.image != null && (businessDTO.image.Equals("string") || businessDTO.image.Length == 0))
                    businessDTO.image = null;

                if (businessDTO.email == null || businessDTO.email.Length == 0 || !await _validationService.CheckEmail(businessDTO.email))
                    throw new InvalidFieldException("Invalid email!!!");

                if (businessDTO.description != null && (businessDTO.description.Equals("string") || businessDTO.description.Length == 0))
                    businessDTO.description = null;

                if (businessDTO.taxIdentificationNumber == null || !await _validationService.CheckText(businessDTO.taxIdentificationNumber))
                    throw new InvalidFieldException("Invalid taxIdentificationNumber!!!");

                if (businessDTO.address == null || !await _validationService.CheckText(businessDTO.address))
                    throw new InvalidFieldException("Invalid address!!!");

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

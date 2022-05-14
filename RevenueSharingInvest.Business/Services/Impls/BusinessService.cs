using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
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
        private readonly IMapper _mapper;
        private readonly String ROLE_ADMIN_ID = "";

        public BusinessService(IBusinessRepository businessRepository, IUserRepository userRepository, IMapper mapper)
        {
            _businessRepository = businessRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        //ADMIN CREATE
        public async Task<int> AdminCreateBusiness(Data.Models.Entities.Business newBusiness, string email)
        {
            User userObject = await _userRepository.GetUserByEmail(email);
            if(userObject == null)
            {
                throw new NotFoundException("User Not Found!!");
            }
            else
            {
                if (!userObject.RoleId.ToString().Equals(ROLE_ADMIN_ID))
                {
                    throw new Exceptions.UnauthorizedAccessException("Only Admin Can Create Business!!");
                }
                else
                {
                    if(await _businessRepository.CreateBusiness(newBusiness) < 1)
                    {
                        throw new CreateBusinessException("Create Business Fail!!");
                    }
                }
            }

            return 1;
        }
        
        //CREATE
        public async Task<int> CreateBusiness(BusinessDTO businessDTO)
        {
            int result;
            try
            {
                RevenueSharingInvest.Data.Models.Entities.Business dto = _mapper.Map<RevenueSharingInvest.Data.Models.Entities.Business>(businessDTO);
                result = await _businessRepository.CreateBusiness(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Business Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
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
                    throw new CreateObjectException("Can not delete Business Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<BusinessDTO>> GetAllBusiness()
        {
            List<RevenueSharingInvest.Data.Models.Entities.Business> businessList = await _businessRepository.GetAllBusiness();
            List<BusinessDTO> list = _mapper.Map<List<BusinessDTO>>(businessList);
            return list;
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
                    throw new CreateObjectException("No Business Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateBusiness(BusinessDTO businessDTO, Guid businessId)
        {
            int result;
            try
            {
                RevenueSharingInvest.Data.Models.Entities.Business dto = _mapper.Map<RevenueSharingInvest.Data.Models.Entities.Business>(businessDTO);
                result = await _businessRepository.UpdateBusiness(dto, businessId);
                if (result == 0)
                    throw new CreateObjectException("Can not update Business Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}

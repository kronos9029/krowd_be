using AutoMapper;
using RevenueSharingInvest.API;
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
using UnauthorizedAccessException = RevenueSharingInvest.Business.Exceptions.UnauthorizedAccessException;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class RiskService : IRiskService
    {
        private readonly IRiskRepository _riskRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public RiskService(IRiskRepository riskRepository, IValidationService validationService, IMapper mapper)
        {
            _riskRepository = riskRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllRiskData()
        {
            int result;
            try
            {
                result = await _riskRepository.ClearAllRiskData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateRisk(RiskDTO riskDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(riskDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (riskDTO.projectId == null || !await _validationService.CheckUUIDFormat(riskDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(riskDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (riskDTO.riskTypeId == null || !await _validationService.CheckUUIDFormat(riskDTO.riskTypeId))
                    throw new InvalidFieldException("Invalid riskTypeId!!!");

                if (!await _validationService.CheckExistenceId("RiskType", Guid.Parse(riskDTO.riskTypeId)))
                    throw new NotFoundException("This riskTypeId is not existed!!!");

                if (riskDTO.description != null && (riskDTO.description.Equals("string") || riskDTO.description.Length == 0))
                    riskDTO.description = null;

                if (riskDTO.createBy != null && riskDTO.createBy.Length >= 0)
                {
                    if (riskDTO.createBy.Equals("string"))
                        riskDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(riskDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (riskDTO.updateBy != null && riskDTO.updateBy.Length >= 0)
                {
                    if (riskDTO.updateBy.Equals("string"))
                        riskDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(riskDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                riskDTO.isDeleted = false;

                Risk dto = _mapper.Map<Risk>(riskDTO);
                newId.id = await _riskRepository.CreateRisk(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Risk Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteRiskById(Guid riskId, ThisUserObj currentUser)
        {
            int result;
            try
            {
                string businessId = await _riskRepository.GetBusinessByRiskId(riskId);

                if (businessId == null)
                    throw new NotFoundException("This Risk Do Not Belong To Any Business!!");
                else if (!businessId.Equals(currentUser.businessId))
                    throw new UnauthorizedAccessException("You Can Not Edit This Risk Information!!");

                result = await _riskRepository.DeleteRiskById(riskId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete Risk Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<RiskDTO>> GetAllRisks(int pageIndex, int pageSize)
        {
            try
            {
                List<Risk> riskList = await _riskRepository.GetAllRisks(pageIndex, pageSize);
                List<RiskDTO> list = _mapper.Map<List<RiskDTO>>(riskList);

                foreach (RiskDTO item in list)
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
        
        public async Task<List<RiskDTO>> GetAllRisksByBusinessId(int pageIndex, int pageSize, string businessId)
        {
            try
            {
                List<Risk> riskList = await _riskRepository.GetAllRisksByBusinessId(pageIndex, pageSize, Guid.Parse(businessId));
                List<RiskDTO> list = _mapper.Map<List<RiskDTO>>(riskList);

                foreach (RiskDTO item in list)
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
        public async Task<RiskDTO> GetRiskById(Guid riskId, ThisUserObj currentUser)
        {
            RiskDTO result;
            try
            {
                string businessId = await _riskRepository.GetBusinessByRiskId(riskId);

                if(businessId == null)
                {
                    throw new NotFoundException("This Risk Have Not Assign To Any Project!!");
                }
                else if (businessId.Equals(currentUser.businessId) || currentUser.investorRoleId.Equals(currentUser.roleId))
                {
                    Risk dto = await _riskRepository.GetRiskById(riskId);
                    result = _mapper.Map<RiskDTO>(dto);
                    if (result == null)
                        throw new NotFoundException("No Risk Object Found!");

                    result.createDate = await _validationService.FormatDateOutput(result.createDate);
                    result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                    return result;
                    
                } else
                {
                    throw new UnauthorizedException("You Do not Have Permisssion To Access This Information!!");
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateRisk(RiskDTO riskDTO, Guid riskId, ThisUserObj currentUser)
        {
            int result;
            try
            {
                string businessId = await _riskRepository.GetBusinessByRiskId(riskId);

                if (businessId == null)
                    throw new NotFoundException("This Risk Do Not Belong To Any Business!!");
                else if (!businessId.Equals(currentUser.businessId))
                    throw new UnauthorizedAccessException("You Can Not Edit This Risk Information!!");



                if (!await _validationService.CheckText(riskDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (riskDTO.projectId == null || !await _validationService.CheckUUIDFormat(riskDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(riskDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (riskDTO.riskTypeId == null || !await _validationService.CheckUUIDFormat(riskDTO.riskTypeId))
                    throw new InvalidFieldException("Invalid riskTypeId!!!");

                if (!await _validationService.CheckExistenceId("RiskType", Guid.Parse(riskDTO.riskTypeId)))
                    throw new NotFoundException("This riskTypeId is not existed!!!");

                if (riskDTO.description != null && (riskDTO.description.Equals("string") || riskDTO.description.Length == 0))
                    riskDTO.description = null;

                if (riskDTO.createBy != null && riskDTO.createBy.Length >= 0)
                {
                    if (riskDTO.createBy.Equals("string"))
                        riskDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(riskDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (riskDTO.updateBy != null && riskDTO.updateBy.Length >= 0)
                {
                    if (riskDTO.updateBy.Equals("string"))
                        riskDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(riskDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                riskDTO.updateDate = currentUser.userId;

                Risk dto = _mapper.Map<Risk>(riskDTO);
                result = await _riskRepository.UpdateRisk(dto, riskId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Risk Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

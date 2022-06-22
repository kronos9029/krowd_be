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
    public class PeriodRevenueService : IPeriodRevenueService
    {
        private readonly IPeriodRevenueRepository _periodRevenueRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public PeriodRevenueService(IPeriodRevenueRepository periodRevenueRepository, IValidationService validationService, IMapper mapper)
        {
            _periodRevenueRepository = periodRevenueRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllPeriodRevenueData()
        {
            int result;
            try
            {
                result = await _periodRevenueRepository.ClearAllPeriodRevenueData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreatePeriodRevenue(PeriodRevenueDTO periodRevenueDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (periodRevenueDTO.projectId == null || !await _validationService.CheckUUIDFormat(periodRevenueDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(periodRevenueDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (periodRevenueDTO.stageId == null || !await _validationService.CheckUUIDFormat(periodRevenueDTO.stageId))
                    throw new InvalidFieldException("Invalid stageId!!!");

                if (!await _validationService.CheckExistenceId("Stage", Guid.Parse(periodRevenueDTO.stageId)))
                    throw new NotFoundException("This stageId is not existed!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                if (!await _validationService.CheckText(periodRevenueDTO.status))
                    throw new InvalidFieldException("Invalid status!!!");

                if (periodRevenueDTO.createBy != null && periodRevenueDTO.createBy.Length >= 0)
                {
                    if (periodRevenueDTO.createBy.Equals("string"))
                        periodRevenueDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(periodRevenueDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (periodRevenueDTO.updateBy != null && periodRevenueDTO.updateBy.Length >= 0)
                {
                    if (periodRevenueDTO.updateBy.Equals("string"))
                        periodRevenueDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(periodRevenueDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                periodRevenueDTO.isDeleted = false;

                PeriodRevenue dto = _mapper.Map<PeriodRevenue>(periodRevenueDTO);
                newId.id = await _periodRevenueRepository.CreatePeriodRevenue(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create PeriodRevenue Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeletePeriodRevenueById(Guid periodRevenueId)
        {
            int result;
            try
            {

                result = await _periodRevenueRepository.DeletePeriodRevenueById(periodRevenueId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete PeriodRevenue Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PeriodRevenueDTO>> GetAllPeriodRevenues(int pageIndex, int pageSize)
        {
            try
            {
                List<PeriodRevenue> periodRevenueList = await _periodRevenueRepository.GetAllPeriodRevenues(pageIndex, pageSize);
                List<PeriodRevenueDTO> list = _mapper.Map<List<PeriodRevenueDTO>>(periodRevenueList);

                foreach (PeriodRevenueDTO item in list)
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
        public async Task<PeriodRevenueDTO> GetPeriodRevenueById(Guid periodRevenueId)
        {
            PeriodRevenueDTO result;
            try
            {
                PeriodRevenue dto = await _periodRevenueRepository.GetPeriodRevenueById(periodRevenueId);
                result = _mapper.Map<PeriodRevenueDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No PeriodRevenue Object Found!");

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
        public async Task<int> UpdatePeriodRevenue(PeriodRevenueDTO periodRevenueDTO, Guid periodRevenueId)
        {
            int result;
            try
            {
                if (periodRevenueDTO.projectId == null || !await _validationService.CheckUUIDFormat(periodRevenueDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(periodRevenueDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (periodRevenueDTO.stageId == null || !await _validationService.CheckUUIDFormat(periodRevenueDTO.stageId))
                    throw new InvalidFieldException("Invalid stageId!!!");

                if (!await _validationService.CheckExistenceId("Stage", Guid.Parse(periodRevenueDTO.stageId)))
                    throw new NotFoundException("This stageId is not existed!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                //if (periodRevenueDTO.investmentTargetCapital <= 0)
                //    throw new InvalidFieldException("investmentTargetCapital must be greater than 0!!!");

                if (!await _validationService.CheckText(periodRevenueDTO.status))
                    throw new InvalidFieldException("Invalid status!!!");

                if (periodRevenueDTO.createBy != null && periodRevenueDTO.createBy.Length >= 0)
                {
                    if (periodRevenueDTO.createBy.Equals("string"))
                        periodRevenueDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(periodRevenueDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (periodRevenueDTO.updateBy != null && periodRevenueDTO.updateBy.Length >= 0)
                {
                    if (periodRevenueDTO.updateBy.Equals("string"))
                        periodRevenueDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(periodRevenueDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                PeriodRevenue dto = _mapper.Map<PeriodRevenue>(periodRevenueDTO);
                result = await _periodRevenueRepository.UpdatePeriodRevenue(dto, periodRevenueId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update PeriodRevenue Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

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
    public class PeriodRevenueService : IPeriodRevenueService
    {
        private readonly IPeriodRevenueRepository _periodRevenueRepository;
        private readonly IMapper _mapper;


        public PeriodRevenueService(IPeriodRevenueRepository periodRevenueRepository, IMapper mapper)
        {
            _periodRevenueRepository = periodRevenueRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreatePeriodRevenue(PeriodRevenueDTO periodRevenueDTO)
        {
            int result;
            try
            {
                PeriodRevenue dto = _mapper.Map<PeriodRevenue>(periodRevenueDTO);
                result = await _periodRevenueRepository.CreatePeriodRevenue(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create PeriodRevenue Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
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
                    throw new CreateObjectException("Can not delete PeriodRevenue Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<PeriodRevenueDTO>> GetAllPeriodRevenues()
        {
            List<PeriodRevenue> periodRevenueList = await _periodRevenueRepository.GetAllPeriodRevenues();
            List<PeriodRevenueDTO> list = _mapper.Map<List<PeriodRevenueDTO>>(periodRevenueList);
            return list;
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
                    throw new CreateObjectException("No PeriodRevenue Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdatePeriodRevenue(PeriodRevenueDTO periodRevenueDTO, Guid periodRevenueId)
        {
            int result;
            try
            {
                PeriodRevenue dto = _mapper.Map<PeriodRevenue>(periodRevenueDTO);
                result = await _periodRevenueRepository.UpdatePeriodRevenue(dto, periodRevenueId);
                if (result == 0)
                    throw new CreateObjectException("Can not update PeriodRevenue Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}

using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
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
    }
}

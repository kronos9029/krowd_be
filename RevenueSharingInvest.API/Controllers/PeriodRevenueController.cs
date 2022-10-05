using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/period_revenues")]
    [EnableCors]
    //[Authorize]
    public class PeriodRevenueController : ControllerBase
    {
        private readonly IPeriodRevenueService _periodRevenueService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public PeriodRevenueController(IPeriodRevenueService periodRevenueService, IHttpContextAccessor httpContextAccessor)
        {
            _periodRevenueService = periodRevenueService;
            this.httpContextAccessor = httpContextAccessor;
        }
    }
}

using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IMoneyOverviewService
    {
        public Task<MoneyOverviewDTO> GetMoneyOverviewForInvestor(ThisUserObj currentUser);
    }
}

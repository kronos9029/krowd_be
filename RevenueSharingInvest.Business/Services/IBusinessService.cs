using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IBusinessService
    {
        public Task<int> AdminCreateBusiness(Data.Models.Entities.Business newBusiness, string email);
    }
}

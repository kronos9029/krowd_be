using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IInvestorWalletRepository
    {
        public Task<float> GetInvestorTotalBalance(String ID);
    }
}

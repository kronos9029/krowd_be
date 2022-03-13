using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.Repos
{
    public class InvestorRepository : BaseRepository, IInvestorRepository
    {
        public InvestorRepository(IConfiguration configuration) : base(configuration)
        {
        }

    }
}

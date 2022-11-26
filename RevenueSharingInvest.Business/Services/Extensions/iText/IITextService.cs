using RevenueSharingInvest.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions.iText
{
    public interface IITextService
    {
        public Task<string> GenerateProjectContract(ThisUserObj currentUser, string projectId, decimal amount, string invesmentId);
    }
}

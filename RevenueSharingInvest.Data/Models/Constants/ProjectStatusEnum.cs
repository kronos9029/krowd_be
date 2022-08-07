using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Models.Constant
{
    public enum ProjectStatusEnum
    {
        DRAFT,
        WAITING_FOR_APPROVAL,
        DENIED,
        ACTIVE,
        CALLING_FOR_INVESTMENT,
        CALLING_TIME_IS_OVER,       
        CLOSED
    }
}

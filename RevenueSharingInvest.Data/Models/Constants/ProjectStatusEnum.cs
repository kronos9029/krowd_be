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
        WAITING_FOR_APPROVED,
        CALLING_FOR_INVESTMENT,
        DENIED,
        CALLING_TIME_IS_OVER,
        WAITING_TO_ACTIVATE,
        ACTIVE,
        CLOSED
    }
}

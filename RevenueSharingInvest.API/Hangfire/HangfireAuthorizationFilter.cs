using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Hangfire
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /*        public bool Authorize([NotNull] DashboardContext context)
                {
                    //var httpContext = context.GetHttpContext();
                    //return httpContext.User.Identity.IsAuthenticated;
                    return true;
                }*/

        public bool Authorize(DashboardContext context) => true;
    }
}

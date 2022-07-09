using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Helpers
{
    public class ClientIpCheckActionFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly string _safelist;

        public ClientIpCheckActionFilter(string safelist, ILogger logger)
        {
            _safelist = safelist;
            _logger = logger;
        }

        private static dynamic GetClientIPAddress(HttpContext context)
        {

            if (!string.IsNullOrEmpty(context.Request.Headers["X-Forwarded-For"]))
            {
                var ip = context.Request.Headers["X-Forwarded-For"];
                return ip;
            }
            else
            {
                var ip = context.Request.HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress;
                return ip;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var Ipv4 = GetClientIPAddress(context.HttpContext);
            var ipSafe = _safelist.Split(';');
            var badIp = true;

            if (Ipv4.IsIPv4MappedToIPv6)
            {
                Ipv4 = Ipv4.MapToIPv4();
            }

            foreach (var address in ipSafe)
            {
                var testIp = IPAddress.Parse(address);

                if (testIp.Equals(Ipv4))
                {
                    badIp = false;
                    break;
                }
            }

            if (badIp)
            {
                //context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                throw new Business.Exceptions.UnauthorizedAccessException("You Can Not Use This End Point!!");
            }

            base.OnActionExecuting(context);
        }
    }
}

using Microsoft.AspNetCore.Http;
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

namespace RevenueSharingInvest.Business.Helpers
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

        private static dynamic GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var Ipv4 = GetLocalIPAddress();
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
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}

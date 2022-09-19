using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Helpers
{
    public class MomoSettings
    {
        public string ApiEndpoint { get;set; }
        public string ApiMobileEndpoint { get;set; }
        public string ApiQueryStatus { get;set; }
        public string ApiConfirmTransaction { get;set; }
        public string PartnerCode { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string ReturnUrl { get; set; }
        public string NotifyUrl { get; set; }
    }
}
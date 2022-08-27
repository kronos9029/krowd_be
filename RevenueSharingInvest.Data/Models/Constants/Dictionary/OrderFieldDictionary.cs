using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.Constants
{
    public static class OrderFieldDictionary
    {
        public static readonly Dictionary<string, string> business = new Dictionary<string, string>
        {
            {"NAME", "Name"},
            {"NUM_OF_PROJECT", "NumOfProject"},
            {"NUM_OF_SUCCESSFUL_PROJECT", "NumOfSuccessfulProject"}
        };

        public static readonly Dictionary<string, string> investor = new Dictionary<string, string>
        {
            {"STATUS", "Status"},
            {"NAME", "Name"}
        };
    }
}

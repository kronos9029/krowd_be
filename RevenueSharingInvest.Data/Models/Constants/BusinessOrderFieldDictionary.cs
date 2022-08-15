using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.Constants
{
    public static class BusinessOrderFieldDictionary
    {
        public static readonly Dictionary<string, string> column = new Dictionary<string, string>
        {
            {"NAME", "Name"},
            {"NUM_OF_PROJECT", "NumOfProject"},
            {"NUM_OF_SUCCESSFUL_PROJECT", "NumOfSuccessfulProject"}
        };
    }
}

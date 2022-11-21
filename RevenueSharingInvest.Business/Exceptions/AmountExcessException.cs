using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Exceptions
{
    public class AmountExcessException : Exception
    {
        public AmountExcessException(string message) : base(message)
        {
        }
    }
}

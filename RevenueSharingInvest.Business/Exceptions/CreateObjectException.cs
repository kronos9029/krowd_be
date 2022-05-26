﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Exceptions
{
    public class CreateObjectException : Exception
    {
        public CreateObjectException(string message) : base(message)
        {
        }
    }
}

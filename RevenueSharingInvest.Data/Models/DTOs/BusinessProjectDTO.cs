using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class BusinessProjectDTO
    {
        public Guid BusinessId { get; private set; }
        public Guid ProjectId { get; private set;}
        public String ProjectName { get; private set; }
        public Guid ManagerId { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class AllProjectPackageDTO
    {
        public int numOfPackage { get; set; }
        public List<GetPackageDTO> listOfPackage { get; set; }
    }
}

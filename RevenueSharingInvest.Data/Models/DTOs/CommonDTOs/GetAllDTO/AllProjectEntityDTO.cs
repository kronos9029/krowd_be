using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class TypeProjectEntityDTO
    {
        public string type { get; set; }
        public List<ProjectComponentProjectEntityDTO> typeItemList { get; set; }
    }
}

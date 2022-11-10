using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class AllProjectManagerWalletDTO
    {
        public GetProjectWalletDTO p1 { get; set; }
        public GetProjectWalletDTO p2 { get; set; }
        public List<GetProjectWalletDTO> p3List { get; set; }
        public List<GetProjectWalletDTO> p4List { get; set; }
        public GetProjectWalletDTO p5 { get; set; }
    }
}

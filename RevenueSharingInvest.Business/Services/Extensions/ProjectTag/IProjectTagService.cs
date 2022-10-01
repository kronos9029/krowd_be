using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions
{
    public interface IProjectTagService
    {
        public Task<List<string>> GetProjectTagList(BasicProjectDTO projectDTO);
    }
}

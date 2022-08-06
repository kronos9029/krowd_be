using AutoMapper;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Common
{
    public class ProjectTagService : IProjectTagService
    {
        private readonly IMapper _mapper;

        public ProjectTagService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<string>> GetProjectTagList(GetProjectDTO projectDTO)
        {
            List<string> tagList = new List<string>();
            try
            {
                if (await NewTag(projectDTO))
                {
                    tagList.Add(Enum.GetNames(typeof(ProjectTagEnum)).ElementAt(0));
                }
                if (await HotTag(projectDTO))
                {
                    tagList.Add(Enum.GetNames(typeof(ProjectTagEnum)).ElementAt(1));
                }
                if (await RecommendedTag(projectDTO))
                {
                    tagList.Add(Enum.GetNames(typeof(ProjectTagEnum)).ElementAt(2));
                }
                return tagList;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private async Task<bool> NewTag(GetProjectDTO projectDTO)
        {
            return true;
        }

        private async Task<bool> HotTag(GetProjectDTO projectDTO)
        {
            return true;
        }

        private async Task<bool> RecommendedTag(GetProjectDTO projectDTO)
        {
            return true;
        }
    }
}

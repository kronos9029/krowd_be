﻿using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IAreaService
    {
        //CREATE
        public Task<IdDTO> CreateArea(AreaDTO areaDTO);

        //READ
        public Task<List<AreaDTO>> GetAllAreas(int pageIndex, int pageSize);
        public Task<AreaDTO> GetAreaById(Guid areaId);

        //UPDATE
        public Task<int> UpdateArea(AreaDTO areaDTO, Guid areaId);

        //DELETE
        public Task<int> DeleteAreaById(Guid areaId);
        public Task<int> ClearAllAreaData();
    }
}

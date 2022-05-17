﻿using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IBusinessFieldRepository
    {
        //CREATE
        public Task<int> CreateBusinessField(BusinessField businessFieldDTO);

        //READ
        public Task<List<BusinessField>> GetAllBusinessFields();
        public Task<BusinessField> GetBusinessFieldById(Guid businessId, Guid fieldId);

        //UPDATE
        public Task<int> UpdateBusinessField(BusinessField businessFieldDTO, Guid businessId, Guid fieldId);

        //DELETE
        public Task<int> DeleteBusinessFieldById(Guid businessId, Guid fieldId);
    }
}

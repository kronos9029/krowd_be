using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IWalletTypeService
    {
        //CREATE

        //READ
        public Task<List<GetWalletTypeDTO>> GetAllWalletTypes();
        public Task<GetWalletTypeDTO> GetWalletTypeById(Guid walletTypeId);

        //UPDATE

        //DELETE
    }
}

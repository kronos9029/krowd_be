using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IProjectWalletService
    {
        //CREATE

        //READ
        public Task<UserWalletsDTO> GetAllProjectWallets(ThisUserObj currentUser);
        public Task<GetProjectWalletDTO> GetAllProjectWalletById(Guid id, ThisUserObj currentUser);

        //UPDATE
        public Task<TransferResponseDTO> TransferBetweenProjectWallet(TransferDTO transferDTO, ThisUserObj currentUser);

        //DELETE
    }
}

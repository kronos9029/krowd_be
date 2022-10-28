using RevenueSharingInvest.API;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IInvestorWalletService
    {
        //CREATE

        //READ
        public Task<UserWalletsDTO> GetAllInvestorWallets(ThisUserObj currentUser);
        public Task<GetInvestorWalletDTO> GetInvestorWalletById(Guid id, ThisUserObj currentUser);

        //UPDATE
        public Task<TransferResponseDTO> TransferBetweenInvestorWallet(TransferDTO transferDTO, ThisUserObj currentUser);

        //DELETE
    }
}

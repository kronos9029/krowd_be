using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IVoucherRepository
    {
        //CREATE
        public Task<string> CreateVoucher(Voucher voucherDTO);

        //READ
        public Task<List<Voucher>> GetAllVouchers(int pageIndex, int pageSize);
        public Task<Voucher> GetVoucherById(Guid voucherId);

        //UPDATE
        public Task<int> UpdateVoucher(Voucher voucherDTO, Guid voucherId);

        //DELETE
        public Task<int> DeleteVoucherById(Guid voucherId);
    }
}

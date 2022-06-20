using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IVoucherItemRepository
    {
        //CREATE
        public Task<string> CreateVoucherItem(VoucherItem voucherItemDTO);

        //READ
        public Task<List<VoucherItem>> GetAllVoucherItems(int pageIndex, int pageSize);
        public Task<VoucherItem> GetVoucherItemById(Guid voucherItemId);

        //UPDATE
        public Task<int> UpdateVoucherItem(VoucherItem voucherItemDTO, Guid voucherItemId);

        //DELETE
        public Task<int> DeleteVoucherItemById(Guid voucherItemId);
        public Task<int> ClearAllVoucherItemData();
    }
}

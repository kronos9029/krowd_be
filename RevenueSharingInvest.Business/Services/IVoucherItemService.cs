using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IVoucherItemService
    {
        //CREATE
        public Task<IdDTO> CreateVoucherItem(VoucherItemDTO voucherItemDTO);

        //READ
        public Task<List<VoucherItemDTO>> GetAllVoucherItems(int pageIndex, int pageSize);
        public Task<VoucherItemDTO> GetVoucherItemById(Guid voucherItemId);

        //UPDATE
        public Task<int> UpdateVoucherItem(VoucherItemDTO voucherItemDTO, Guid voucherItemId);

        //DELETE
        public Task<int> DeleteVoucherItemById(Guid voucherItemId);
        public Task<int> ClearAllVoucherItemData();
    }
}

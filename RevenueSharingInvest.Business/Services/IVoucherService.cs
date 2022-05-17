using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IVoucherService
    {
        //CREATE
        public Task<int> CreateVoucher(VoucherDTO voucherDTO);

        //READ
        public Task<List<VoucherDTO>> GetAllVouchers();
        public Task<VoucherDTO> GetVoucherById(Guid voucherId);

        //UPDATE
        public Task<int> UpdateVoucher(VoucherDTO voucherDTO, Guid voucherId);

        //DELETE
        public Task<int> DeleteVoucherById(Guid voucherId);
    }
}

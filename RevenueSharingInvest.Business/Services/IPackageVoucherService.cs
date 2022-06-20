using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services
{
    public interface IPackageVoucherService
    {
        //CREATE
        public Task<int> CreatePackageVoucher(PackageVoucherDTO packageVoucherDTO);

        //READ
        public Task<List<PackageVoucherDTO>> GetAllPackageVouchers(int pageIndex, int pageSize);
        public Task<PackageVoucherDTO> GetPackageVoucherById(Guid packageId, Guid voucherId);

        //UPDATE
        public Task<int> UpdatePackageVoucher(PackageVoucherDTO packageVoucherDTO, Guid packageId, Guid voucherId);

        //DELETE
        public Task<int> DeletePackageVoucherById(Guid packageId, Guid voucherId);
        public Task<int> ClearAllPackageVoucherData();
    }
}

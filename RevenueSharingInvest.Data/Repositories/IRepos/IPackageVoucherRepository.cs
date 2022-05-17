using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.IRepos
{
    public interface IPackageVoucherRepository
    {
        //CREATE
        public Task<int> CreatePackageVoucher(PackageVoucher packageVoucherDTO);

        //READ
        public Task<List<PackageVoucher>> GetAllPackageVouchers();
        public Task<PackageVoucher> GetPackageVoucherById(Guid packageId, Guid voucherId);

        //UPDATE
        public Task<int> UpdatePackageVoucher(PackageVoucher packageVoucherDTO, Guid packageId, Guid voucherId);

        //DELETE
        public Task<int> DeletePackageVoucherById(Guid packageId, Guid voucherId);
    }
}

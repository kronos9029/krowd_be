using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class PackageVoucherService : IPackageVoucherService
    {
        private readonly IPackageVoucherRepository _packageVoucherRepository;
        private readonly IMapper _mapper;


        public PackageVoucherService(IPackageVoucherRepository packageVoucherRepository, IMapper mapper)
        {
            _packageVoucherRepository = packageVoucherRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreatePackageVoucher(PackageVoucherDTO packageVoucherDTO)
        {
            int result;
            try
            {
                PackageVoucher dto = _mapper.Map<PackageVoucher>(packageVoucherDTO);
                result = await _packageVoucherRepository.CreatePackageVoucher(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create PackageVoucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeletePackageVoucherById(Guid packageId, Guid voucherId)
        {
            int result;
            try
            {

                result = await _packageVoucherRepository.DeletePackageVoucherById(packageId, voucherId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete PackageVoucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<PackageVoucherDTO>> GetAllPackageVouchers()
        {
            List<PackageVoucher> packageVoucherList = await _packageVoucherRepository.GetAllPackageVouchers();
            List<PackageVoucherDTO> list = _mapper.Map<List<PackageVoucherDTO>>(packageVoucherList);
            return list;
        }

        //GET BY ID
        public async Task<PackageVoucherDTO> GetPackageVoucherById(Guid packageId, Guid voucherId)
        {
            PackageVoucherDTO result;
            try
            {

                PackageVoucher dto = await _packageVoucherRepository.GetPackageVoucherById(packageId, voucherId);
                result = _mapper.Map<PackageVoucherDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No PackageVoucher Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdatePackageVoucher(PackageVoucherDTO packageVoucherDTO, Guid packageId, Guid voucherId)
        {
            int result;
            try
            {
                PackageVoucher dto = _mapper.Map<PackageVoucher>(packageVoucherDTO);
                result = await _packageVoucherRepository.UpdatePackageVoucher(dto, packageId, voucherId);
                if (result == 0)
                    throw new CreateObjectException("Can not update PackageVoucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}

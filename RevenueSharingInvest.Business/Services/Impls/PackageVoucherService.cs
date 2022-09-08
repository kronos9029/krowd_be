using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
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
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public PackageVoucherService(IPackageVoucherRepository packageVoucherRepository, IValidationService validationService, IMapper mapper)
        {
            _packageVoucherRepository = packageVoucherRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllPackageVoucherData()
        {
            int result;
            try
            {
                result = await _packageVoucherRepository.ClearAllPackageVoucherData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<int> CreatePackageVoucher(PackageVoucherDTO packageVoucherDTO)
        {
            int result;
            try
            {
                if (packageVoucherDTO.packageId == null || !await _validationService.CheckUUIDFormat(packageVoucherDTO.packageId))
                    throw new InvalidFieldException("Invalid packageId!!!");

                if (!await _validationService.CheckExistenceId("Package", Guid.Parse(packageVoucherDTO.packageId)))
                    throw new NotFoundException("This packageId is not existed!!!");

                if (packageVoucherDTO.voucherId == null || !await _validationService.CheckUUIDFormat(packageVoucherDTO.voucherId))
                    throw new InvalidFieldException("Invalid voucherId!!!");

                if (!await _validationService.CheckExistenceId("Voucher", Guid.Parse(packageVoucherDTO.voucherId)))
                    throw new NotFoundException("This voucherId is not existed!!!");

                if (packageVoucherDTO.quantity <= 0)
                    throw new InvalidFieldException("quantity must be greater than 0!!!");

                if (packageVoucherDTO.maxQuantity <= 0)
                    throw new InvalidFieldException("maxQuantity must be greater than 0!!!");

                if (packageVoucherDTO.createBy != null && packageVoucherDTO.createBy.Length >= 0)
                {
                    if (packageVoucherDTO.createBy.Equals("string"))
                        packageVoucherDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(packageVoucherDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (packageVoucherDTO.updateBy != null && packageVoucherDTO.updateBy.Length >= 0)
                {
                    if (packageVoucherDTO.updateBy.Equals("string"))
                        packageVoucherDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(packageVoucherDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                packageVoucherDTO.isDeleted = false;

                PackageVoucher dto = _mapper.Map<PackageVoucher>(packageVoucherDTO);
                result = await _packageVoucherRepository.CreatePackageVoucher(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create PackageVoucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete PackageVoucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<PackageVoucherDTO>> GetAllPackageVouchers(int pageIndex, int pageSize)
        {
            try
            {
                List<PackageVoucher> packageVoucherList = await _packageVoucherRepository.GetAllPackageVouchers(pageIndex, pageSize);
                List<PackageVoucherDTO> list = _mapper.Map<List<PackageVoucherDTO>>(packageVoucherList);

                foreach (PackageVoucherDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);
                }

                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
                    throw new NotFoundException("No PackageVoucher Object Found!");

                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdatePackageVoucher(PackageVoucherDTO packageVoucherDTO, Guid packageId, Guid voucherId)
        {
            int result;
            try
            {
                if (packageVoucherDTO.packageId == null || !await _validationService.CheckUUIDFormat(packageVoucherDTO.packageId))
                    throw new InvalidFieldException("Invalid packageId!!!");

                if (!await _validationService.CheckExistenceId("Package", Guid.Parse(packageVoucherDTO.packageId)))
                    throw new NotFoundException("This packageId is not existed!!!");

                if (packageVoucherDTO.voucherId == null || !await _validationService.CheckUUIDFormat(packageVoucherDTO.voucherId))
                    throw new InvalidFieldException("Invalid voucherId!!!");

                if (!await _validationService.CheckExistenceId("Voucher", Guid.Parse(packageVoucherDTO.voucherId)))
                    throw new NotFoundException("This voucherId is not existed!!!");

                if (packageVoucherDTO.quantity <= 0)
                    throw new InvalidFieldException("quantity must be greater than 0!!!");

                if (packageVoucherDTO.maxQuantity <= 0)
                    throw new InvalidFieldException("maxQuantity must be greater than 0!!!");

                if (packageVoucherDTO.createBy != null && packageVoucherDTO.createBy.Length >= 0)
                {
                    if (packageVoucherDTO.createBy.Equals("string"))
                        packageVoucherDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(packageVoucherDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (packageVoucherDTO.updateBy != null && packageVoucherDTO.updateBy.Length >= 0)
                {
                    if (packageVoucherDTO.updateBy.Equals("string"))
                        packageVoucherDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(packageVoucherDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                PackageVoucher dto = _mapper.Map<PackageVoucher>(packageVoucherDTO);
                result = await _packageVoucherRepository.UpdatePackageVoucher(dto, packageId, voucherId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update PackageVoucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

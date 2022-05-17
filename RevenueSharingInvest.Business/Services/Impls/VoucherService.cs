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
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMapper _mapper;


        public VoucherService(IVoucherRepository voucherRepository, IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateVoucher(VoucherDTO voucherDTO)
        {
            int result;
            try
            {
                Voucher dto = _mapper.Map<Voucher>(voucherDTO);
                result = await _voucherRepository.CreateVoucher(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Voucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteVoucherById(Guid voucherId)
        {
            int result;
            try
            {

                result = await _voucherRepository.DeleteVoucherById(voucherId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete Voucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<VoucherDTO>> GetAllVouchers()
        {
            List<Voucher> voucherList = await _voucherRepository.GetAllVouchers();
            List<VoucherDTO> list = _mapper.Map<List<VoucherDTO>>(voucherList);
            return list;
        }

        //GET BY ID
        public async Task<VoucherDTO> GetVoucherById(Guid voucherId)
        {
            VoucherDTO result;
            try
            {

                Voucher dto = await _voucherRepository.GetVoucherById(voucherId);
                result = _mapper.Map<VoucherDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No Voucher Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateVoucher(VoucherDTO voucherDTO, Guid voucherId)
        {
            int result;
            try
            {
                Voucher dto = _mapper.Map<Voucher>(voucherDTO);
                result = await _voucherRepository.UpdateVoucher(dto, voucherId);
                if (result == 0)
                    throw new CreateObjectException("Can not update Voucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}

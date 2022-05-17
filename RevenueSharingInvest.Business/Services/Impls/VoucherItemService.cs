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
    public class VoucherItemService : IVoucherItemService
    {
        private readonly IVoucherItemRepository _voucherItemRepository;
        private readonly IMapper _mapper;


        public VoucherItemService(IVoucherItemRepository voucherItemRepository, IMapper mapper)
        {
            _voucherItemRepository = voucherItemRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateVoucherItem(VoucherItemDTO voucherItemDTO)
        {
            int result;
            try
            {
                VoucherItem dto = _mapper.Map<VoucherItem>(voucherItemDTO);
                result = await _voucherItemRepository.CreateVoucherItem(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create VoucherItem Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteVoucherItemById(Guid voucherItemId)
        {
            int result;
            try
            {

                result = await _voucherItemRepository.DeleteVoucherItemById(voucherItemId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete VoucherItem Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<VoucherItemDTO>> GetAllVoucherItems()
        {
            List<VoucherItem> voucherItemList = await _voucherItemRepository.GetAllVoucherItems();
            List<VoucherItemDTO> list = _mapper.Map<List<VoucherItemDTO>>(voucherItemList);
            return list;
        }

        //GET BY ID
        public async Task<VoucherItemDTO> GetVoucherItemById(Guid voucherItemId)
        {
            VoucherItemDTO result;
            try
            {

                VoucherItem dto = await _voucherItemRepository.GetVoucherItemById(voucherItemId);
                result = _mapper.Map<VoucherItemDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No VoucherItem Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateVoucherItem(VoucherItemDTO voucherItemDTO, Guid voucherItemId)
        {
            int result;
            try
            {
                VoucherItem dto = _mapper.Map<VoucherItem>(voucherItemDTO);
                result = await _voucherItemRepository.UpdateVoucherItem(dto, voucherItemId);
                if (result == 0)
                    throw new CreateObjectException("Can not update VoucherItem Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}

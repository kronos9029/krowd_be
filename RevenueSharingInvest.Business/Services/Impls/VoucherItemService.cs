using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Common;
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
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public VoucherItemService(IVoucherItemRepository voucherItemRepository, IValidationService validationService, IMapper mapper)
        {
            _voucherItemRepository = voucherItemRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllVoucherItemData()
        {
            int result;
            try
            {
                result = await _voucherItemRepository.ClearAllVoucherItemData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateVoucherItem(VoucherItemDTO voucherItemDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (voucherItemDTO.voucherId == null || !await _validationService.CheckUUIDFormat(voucherItemDTO.voucherId))
                    throw new InvalidFieldException("Invalid voucherId!!!");

                if (!await _validationService.CheckExistenceId("Voucher", Guid.Parse(voucherItemDTO.voucherId)))
                    throw new NotFoundException("This voucherId is not existed!!!");

                if (voucherItemDTO.investmentId == null || !await _validationService.CheckUUIDFormat(voucherItemDTO.investmentId))
                    throw new InvalidFieldException("Invalid investmentId!!!");

                if (!await _validationService.CheckExistenceId("Investment", Guid.Parse(voucherItemDTO.investmentId)))
                    throw new NotFoundException("This investmentId is not existed!!!");


                if (!await _validationService.CheckDate((voucherItemDTO.issuedDate)))
                    throw new InvalidFieldException("Invalid issuedDate!!!");

                voucherItemDTO.issuedDate = await _validationService.FormatDateInput(voucherItemDTO.issuedDate);

                if (!await _validationService.CheckDate((voucherItemDTO.expireDate)))
                    throw new InvalidFieldException("Invalid expireDate!!!");

                voucherItemDTO.expireDate = await _validationService.FormatDateInput(voucherItemDTO.expireDate);

                if (!await _validationService.CheckDate((voucherItemDTO.redeemDate)))
                    throw new InvalidFieldException("Invalid redeemDate!!!");

                voucherItemDTO.redeemDate = await _validationService.FormatDateInput(voucherItemDTO.redeemDate);

                if (!await _validationService.CheckDate((voucherItemDTO.availableDate)))
                    throw new InvalidFieldException("Invalid availableDate!!!");

                voucherItemDTO.availableDate = await _validationService.FormatDateInput(voucherItemDTO.availableDate);

                if (voucherItemDTO.createBy != null && voucherItemDTO.createBy.Length >= 0)
                {
                    if (voucherItemDTO.createBy.Equals("string"))
                        voucherItemDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(voucherItemDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (voucherItemDTO.updateBy != null && voucherItemDTO.updateBy.Length >= 0)
                {
                    if (voucherItemDTO.updateBy.Equals("string"))
                        voucherItemDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(voucherItemDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                voucherItemDTO.isDeleted = false;

                VoucherItem dto = _mapper.Map<VoucherItem>(voucherItemDTO);
                newId.id = await _voucherItemRepository.CreateVoucherItem(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create VoucherItem Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete VoucherItem Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<VoucherItemDTO>> GetAllVoucherItems(int pageIndex, int pageSize)
        {
            try
            {
                List<VoucherItem> voucherItemList = await _voucherItemRepository.GetAllVoucherItems(pageIndex, pageSize);
                List<VoucherItemDTO> list = _mapper.Map<List<VoucherItemDTO>>(voucherItemList);

                foreach (VoucherItemDTO item in list)
                {
                    item.issuedDate = await _validationService.FormatDateOutput(item.issuedDate);
                    item.expireDate = await _validationService.FormatDateOutput(item.expireDate);
                    item.redeemDate = await _validationService.FormatDateOutput(item.redeemDate);
                    item.availableDate = await _validationService.FormatDateOutput(item.availableDate);
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
        public async Task<VoucherItemDTO> GetVoucherItemById(Guid voucherItemId)
        {
            VoucherItemDTO result;
            try
            {
                VoucherItem dto = await _voucherItemRepository.GetVoucherItemById(voucherItemId);
                result = _mapper.Map<VoucherItemDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No VoucherItem Object Found!");

                result.issuedDate = await _validationService.FormatDateOutput(result.issuedDate);
                result.expireDate = await _validationService.FormatDateOutput(result.expireDate);
                result.redeemDate = await _validationService.FormatDateOutput(result.redeemDate);
                result.availableDate = await _validationService.FormatDateOutput(result.availableDate);
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
        public async Task<int> UpdateVoucherItem(VoucherItemDTO voucherItemDTO, Guid voucherItemId)
        {
            int result;
            try
            {
                if (voucherItemDTO.voucherId == null || !await _validationService.CheckUUIDFormat(voucherItemDTO.voucherId))
                    throw new InvalidFieldException("Invalid voucherId!!!");

                if (!await _validationService.CheckExistenceId("Voucher", Guid.Parse(voucherItemDTO.voucherId)))
                    throw new NotFoundException("This voucherId is not existed!!!");

                if (voucherItemDTO.investmentId == null || !await _validationService.CheckUUIDFormat(voucherItemDTO.investmentId))
                    throw new InvalidFieldException("Invalid investmentId!!!");

                if (!await _validationService.CheckExistenceId("Investment", Guid.Parse(voucherItemDTO.investmentId)))
                    throw new NotFoundException("This investmentId is not existed!!!");

                if (!await _validationService.CheckDate((voucherItemDTO.issuedDate)))
                    throw new InvalidFieldException("Invalid issuedDate!!!");

                voucherItemDTO.issuedDate = await _validationService.FormatDateInput(voucherItemDTO.issuedDate);

                if (!await _validationService.CheckDate((voucherItemDTO.expireDate)))
                    throw new InvalidFieldException("Invalid expireDate!!!");

                voucherItemDTO.expireDate = await _validationService.FormatDateInput(voucherItemDTO.expireDate);

                if (!await _validationService.CheckDate((voucherItemDTO.redeemDate)))
                    throw new InvalidFieldException("Invalid redeemDate!!!");

                voucherItemDTO.redeemDate = await _validationService.FormatDateInput(voucherItemDTO.redeemDate);

                if (!await _validationService.CheckDate((voucherItemDTO.availableDate)))
                    throw new InvalidFieldException("Invalid availableDate!!!");

                voucherItemDTO.availableDate = await _validationService.FormatDateInput(voucherItemDTO.availableDate);

                if (voucherItemDTO.createBy != null && voucherItemDTO.createBy.Length >= 0)
                {
                    if (voucherItemDTO.createBy.Equals("string"))
                        voucherItemDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(voucherItemDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (voucherItemDTO.updateBy != null && voucherItemDTO.updateBy.Length >= 0)
                {
                    if (voucherItemDTO.updateBy.Equals("string"))
                        voucherItemDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(voucherItemDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                VoucherItem dto = _mapper.Map<VoucherItem>(voucherItemDTO);
                result = await _voucherItemRepository.UpdateVoucherItem(dto, voucherItemId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update VoucherItem Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

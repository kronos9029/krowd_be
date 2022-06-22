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
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public VoucherService(IVoucherRepository voucherRepository, IValidationService validationService, IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CLEAR DATA
        public async Task<int> ClearAllVoucherData()
        {
            int result;
            try
            {
                result = await _voucherRepository.ClearAllVoucherData();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //CREATE
        public async Task<IdDTO> CreateVoucher(VoucherDTO voucherDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (voucherDTO.projectId == null || !await _validationService.CheckUUIDFormat(voucherDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(voucherDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (!await _validationService.CheckText(voucherDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (!await _validationService.CheckText(voucherDTO.code))
                    throw new InvalidFieldException("Invalid code!!!");

                if (voucherDTO.description != null && (voucherDTO.description.Equals("string") || voucherDTO.description.Length == 0))
                    voucherDTO.description = null;

                if (voucherDTO.image != null && (voucherDTO.image.Equals("string") || voucherDTO.image.Length == 0))
                    voucherDTO.image = null;

                if (voucherDTO.quantity <= 0)
                    throw new InvalidFieldException("quantity must be greater than 0!!!");

                if (!await _validationService.CheckText(voucherDTO.status))
                    throw new InvalidFieldException("Invalid firstName!!!");

                if (!await _validationService.CheckDate((voucherDTO.startDate)))
                    throw new InvalidFieldException("Invalid startDate!!!");

                voucherDTO.startDate = await _validationService.FormatDateInput(voucherDTO.startDate);

                if (!await _validationService.CheckDate((voucherDTO.endDate)))
                    throw new InvalidFieldException("Invalid endDate!!!");

                voucherDTO.endDate = await _validationService.FormatDateInput(voucherDTO.endDate);

                if (voucherDTO.createBy != null && voucherDTO.createBy.Length >= 0)
                {
                    if (voucherDTO.createBy.Equals("string"))
                        voucherDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(voucherDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (voucherDTO.updateBy != null && voucherDTO.updateBy.Length >= 0)
                {
                    if (voucherDTO.updateBy.Equals("string"))
                        voucherDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(voucherDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                voucherDTO.isDeleted = false;

                Voucher dto = _mapper.Map<Voucher>(voucherDTO);
                newId.id = await _voucherRepository.CreateVoucher(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Voucher Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                    throw new DeleteObjectException("Can not delete Voucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<VoucherDTO>> GetAllVouchers(int pageIndex, int pageSize)
        {
            try
            {
                List<Voucher> voucherList = await _voucherRepository.GetAllVouchers(pageIndex, pageSize);
                List<VoucherDTO> list = _mapper.Map<List<VoucherDTO>>(voucherList);

                foreach (VoucherDTO item in list)
                {
                    item.startDate = await _validationService.FormatDateOutput(item.startDate);
                    item.endDate = await _validationService.FormatDateOutput(item.endDate);
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
        public async Task<VoucherDTO> GetVoucherById(Guid voucherId)
        {
            VoucherDTO result;
            try
            {
                Voucher dto = await _voucherRepository.GetVoucherById(voucherId);
                result = _mapper.Map<VoucherDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No Voucher Object Found!");

                result.startDate = await _validationService.FormatDateOutput(result.startDate);
                result.endDate = await _validationService.FormatDateOutput(result.endDate);
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
        public async Task<int> UpdateVoucher(VoucherDTO voucherDTO, Guid voucherId)
        {
            int result;
            try
            {
                if (voucherDTO.projectId == null || !await _validationService.CheckUUIDFormat(voucherDTO.projectId))
                    throw new InvalidFieldException("Invalid projectId!!!");

                if (!await _validationService.CheckExistenceId("Project", Guid.Parse(voucherDTO.projectId)))
                    throw new NotFoundException("This projectId is not existed!!!");

                if (!await _validationService.CheckText(voucherDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (!await _validationService.CheckText(voucherDTO.code))
                    throw new InvalidFieldException("Invalid code!!!");

                if (voucherDTO.description != null && (voucherDTO.description.Equals("string") || voucherDTO.description.Length == 0))
                    voucherDTO.description = null;

                if (voucherDTO.image != null && (voucherDTO.image.Equals("string") || voucherDTO.image.Length == 0))
                    voucherDTO.image = null;

                if (voucherDTO.quantity <= 0)
                    throw new InvalidFieldException("quantity must be greater than 0!!!");

                if (!await _validationService.CheckText(voucherDTO.status))
                    throw new InvalidFieldException("Invalid firstName!!!");

                if (!await _validationService.CheckDate((voucherDTO.startDate)))
                    throw new InvalidFieldException("Invalid startDate!!!");

                voucherDTO.startDate = await _validationService.FormatDateInput(voucherDTO.startDate);

                if (!await _validationService.CheckDate((voucherDTO.endDate)))
                    throw new InvalidFieldException("Invalid endDate!!!");

                voucherDTO.endDate = await _validationService.FormatDateInput(voucherDTO.endDate);

                if (voucherDTO.createBy != null && voucherDTO.createBy.Length >= 0)
                {
                    if (voucherDTO.createBy.Equals("string"))
                        voucherDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(voucherDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (voucherDTO.updateBy != null && voucherDTO.updateBy.Length >= 0)
                {
                    if (voucherDTO.updateBy.Equals("string"))
                        voucherDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(voucherDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                Voucher dto = _mapper.Map<Voucher>(voucherDTO);
                result = await _voucherRepository.UpdateVoucher(dto, voucherId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Voucher Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

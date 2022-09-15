using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly String ROLE_ADMIN_ID = "ff54acc6-c4e9-4b73-a158-fd640b4b6940";


        public RoleService(IRoleRepository roleRepository, IValidationService validationService, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //CREATE
        public async Task<IdDTO> CreateRole(RoleDTO roleDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
                if (!await _validationService.CheckText(roleDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (roleDTO.description != null && (roleDTO.description.Equals("string") || roleDTO.description.Length == 0))
                    roleDTO.description = null;

                if (roleDTO.createBy != null && roleDTO.createBy.Length >= 0)
                {
                    if (roleDTO.createBy.Equals("string"))
                        roleDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(roleDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (roleDTO.updateBy != null && roleDTO.updateBy.Length >= 0)
                {
                    if (roleDTO.updateBy.Equals("string"))
                        roleDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(roleDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                roleDTO.isDeleted = false;

                Role dto = _mapper.Map<Role>(roleDTO);
                newId.id = await _roleRepository.CreateRole(dto);
                if (newId.id.Equals(""))
                    throw new CreateObjectException("Can not create Role Object!");
                return newId;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE
        public async Task<int> DeleteRoleById(Guid roleId)
        {
            int result;
            try
            {

                result = await _roleRepository.DeleteRoleById(roleId);
                if (result == 0)
                    throw new DeleteObjectException("Can not delete Role Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<RoleDTO>> GetAllRoles()
        {
            try
            {
                List<Role> roleList = await _roleRepository.GetAllRoles();
                List<RoleDTO> list = _mapper.Map<List<RoleDTO>>(roleList);

                foreach (RoleDTO item in list)
                {
                    if (item.createDate != null && item.updateDate != null) 
                    {
                        item.createDate = await _validationService.FormatDateOutput(item.createDate);
                        item.updateDate = await _validationService.FormatDateOutput(item.updateDate);
                    }

                }

                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<RoleDTO> GetRoleById(Guid roleId)
        {
            RoleDTO result;
            try
            {

                Role dto = await _roleRepository.GetRoleById(roleId);
                result = _mapper.Map<RoleDTO>(dto);
                if (result == null)
                    throw new NotFoundException("No Role Object Found!");

                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (NotFoundException e)
            {
                throw new Exception(e.Message);
            }
        }

        //UPDATE
        public async Task<int> UpdateRole(RoleDTO roleDTO, Guid roleId)
        {
            int result;
            try
            {
                if (!await _validationService.CheckText(roleDTO.name))
                    throw new InvalidFieldException("Invalid name!!!");

                if (roleDTO.description != null && (roleDTO.description.Equals("string") || roleDTO.description.Length == 0))
                    roleDTO.description = null;

                if (roleDTO.createBy != null && roleDTO.createBy.Length >= 0)
                {
                    if (roleDTO.createBy.Equals("string"))
                        roleDTO.createBy = null;
                    else if (!await _validationService.CheckUUIDFormat(roleDTO.createBy))
                        throw new InvalidFieldException("Invalid createBy!!!");
                }

                if (roleDTO.updateBy != null && roleDTO.updateBy.Length >= 0)
                {
                    if (roleDTO.updateBy.Equals("string"))
                        roleDTO.updateBy = null;
                    else if (!await _validationService.CheckUUIDFormat(roleDTO.updateBy))
                        throw new InvalidFieldException("Invalid updateBy!!!");
                }

                Role dto = _mapper.Map<Role>(roleDTO);
                result = await _roleRepository.UpdateRole(dto, roleId);
                if (result == 0)
                    throw new UpdateObjectException("Can not update Role Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

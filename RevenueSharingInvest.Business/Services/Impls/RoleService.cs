using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
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
        private readonly IMapper _mapper;


        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<IdDTO> CreateRole(RoleDTO roleDTO)
        {
            IdDTO newId = new IdDTO();
            try
            {
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
            List<Role> roleList = await _roleRepository.GetAllRoles();
            List<RoleDTO> list = _mapper.Map<List<RoleDTO>>(roleList);
            return list;
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

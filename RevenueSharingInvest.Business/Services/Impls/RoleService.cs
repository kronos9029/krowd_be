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
        public async Task<int> CreateRole(RoleDTO roleDTO)
        {
            int result;
            try
            {
                Role dto = _mapper.Map<Role>(roleDTO);
                result = await _roleRepository.CreateRole(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create Role Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
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
                    throw new CreateObjectException("Can not delete Role Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
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
                    throw new CreateObjectException("No Role Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
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
                    throw new CreateObjectException("Can not update Role Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}

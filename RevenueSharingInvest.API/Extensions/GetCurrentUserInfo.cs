﻿using Microsoft.AspNetCore.Http;
using RevenueSharingInvest.Data.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using RevenueSharingInvest.Business.Services;
using System.Security.Claims;
using RevenueSharingInvest.Business.Models.Constant;

namespace RevenueSharingInvest.API.Extensions
{
    internal static class GetCurrentUserInfo
    {

        internal static async Task<ThisUserObj> GetThisUserInfo(HttpContext httpContext, IRoleService _roleService, IUserService _userService)
        {
            ThisUserObj currentUser = new();

            var checkUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber);
            if (checkUser == null)
            {
                currentUser.userId = "";
                currentUser.email = "";
                currentUser.investorId = "";
                currentUser.roleName = "";
                currentUser.fullName = "";
            }
            else
            {
                currentUser.userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;
                currentUser.email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                currentUser.investorId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GroupSid).Value;
                currentUser.roleName = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
                currentUser.fullName = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor).Value;
            }

            List<RoleDTO> roleList = await _roleService.GetAllRoles();
            GetUserDTO? userDTO = await _userService.GetUserByEmail(currentUser.email);

            if (userDTO == null)
            {
                currentUser.roleId = "";
                currentUser.businessId = "";
            }
            else
            {
                currentUser.roleId = userDTO.role.id;
                
                if (userDTO.business != null)
                {
                    currentUser.businessId = userDTO.business.id;
                }
                else
                {
                    currentUser.businessId = "";
                }
            }

            foreach (RoleDTO role in roleList)
            {
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(0)))
                {
                    currentUser.adminRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(3)))
                {
                    currentUser.investorRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(1)))
                {
                    currentUser.businessManagerRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(2)))
                {
                    currentUser.projectManagerRoleId = role.id;
                }
            }

            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                //currentUser.projectId = await _userService.GetProjectIdByManagerEmail(currentUser.email);
                //currentUser.projectId??="";

            }

            return currentUser;
        }
    }
}

﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/users")]
    [EnableCors]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
        {
            var result = await _userService.CreateUser(userDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = new List<UserDTO>();
            result = await _userService.GetAllUsers();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            UserDTO dto = new UserDTO();
            dto = await _userService.GetUserById(id);
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO userDTO, [FromQuery] Guid userId)
        {
            var result = await _userService.UpdateUser(userDTO, userId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllUserData()
        {
            var result = await _userService.ClearAllUserData();
            return Ok(result);
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("authenticate-mobile")]
        //public async Task<IActionResult> AuthenticateMobile([FromQuery]string token)
        //{
        //    var result = await _userService.GetTokenInvestor(token);
        //    return Ok(result);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("authenticate-web")]
        //public async Task<IActionResult> AuthenticateWeb([FromQuery]string token)
        //{
        //    var result = await _userService.GetTokenWebBusiness(token);
        //    return Ok(result);
        //}

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/upload-files")]
    [EnableCors]
    [Authorize]
    public class UploadFileController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly IBusinessService _businessService;

        public UploadFileController(IFileUploadService fileUploadService, IRoleService roleService, IUserService userService, IBusinessService businessService)
        {
            _fileUploadService = fileUploadService;
            _roleService = roleService;
            _userService = userService;
            _businessService = businessService;
        }

        [HttpPost]
        [Route("firebase")]
        public async Task<IActionResult> UploadFileToFirebase([FromForm] FirebaseRequest firebaseRequest)
        {
            ThisUserObj userInfo = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            firebaseRequest.createBy = userInfo.userId;
            firebaseRequest.businessId = userInfo.businessId;

            var result = await _fileUploadService.UploadFilesWithPath(firebaseRequest);
            return Ok(result);
        }

        [HttpPost]
        [Route("contract")]
        public async Task<IActionResult> UploadContractToFirebase([FromForm] IFormFile file)
        {
            ThisUserObj userInfo = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            FirebaseBusinessContract firebaseBusinessContract = new();
            firebaseBusinessContract.businessId = userInfo.businessId.ToString();
            firebaseBusinessContract.businessName = await _businessService.GetBusinessNameById(userInfo.businessId);
            firebaseBusinessContract.projectOwnerId = userInfo.userId;
            firebaseBusinessContract.projectOwnerEmail = userInfo.email;
            var result = _fileUploadService.UploadBusinessContract(firebaseBusinessContract);
            return Ok(result);
        }
        
        [HttpPost]
        [Route("excel")]
        public async Task<IActionResult> UploadExcel([FromForm] FirebaseRequest firebaseRequest)
        {
            ThisUserObj userInfo = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            return Ok(0);
        


        /*        [HttpDelete]
                public async Task<IActionResult> DeleteImagesFromFirebase(FirebaseEntity firebaseEntity)
                {
                    return Ok(_fileUploadService.DeleteImagesFromFirebase(firebaseEntity));
                }*/
    }
}

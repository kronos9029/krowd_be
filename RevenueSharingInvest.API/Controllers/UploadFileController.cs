using ExcelDataReader;
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
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/upload-files")]
    [EnableCors]
    //[Authorize]
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
        public async Task<IActionResult> UploadContractToFirebase(IFormFile file)
        {
            ThisUserObj userInfo = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            FirebaseBusinessContract firebaseBusinessContract = new();
            firebaseBusinessContract.businessId = userInfo.businessId.ToString();
            firebaseBusinessContract.businessName = await _businessService.GetBusinessNameById(userInfo.businessId);
            firebaseBusinessContract.projectOwnerId = userInfo.userId;
            firebaseBusinessContract.projectOwnerEmail = userInfo.email;
            var result = await _fileUploadService.UploadBusinessContract(firebaseBusinessContract);
            return Ok(result);
        }

/*        [HttpPost]
        [Route("excel")]
        public async Task<IActionResult> UploadExcel([FromForm] IFormFile excelFile)
        {
            ThisUserObj userInfo = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            string ok = await _fileUploadService.ExcelFileReader(excelFile);
            return Ok(0);


        }*/

        [HttpPost]
        [Route("excel")]
        public async Task<IActionResult> UploadExcel(IFormFile excelFile)
        {
            try
            {
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                Stream FileStream = excelFile.OpenReadStream();

                if (excelFile != null && FileStream != null)
                {
                    if (excelFile.FileName.EndsWith(".xls"))
                        reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                    else if (excelFile.FileName.EndsWith(".xlsx"))
                        reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);

                    dsexcelRecords = reader.AsDataSet();
                    reader.Close();
                }

                return Ok(dsexcelRecords);
            }
            catch (Exception)
            {
                throw;
            }

        }
        /*        [HttpDelete]
                public async Task<IActionResult> DeleteImagesFromFirebase(FirebaseEntity firebaseEntity)
                {
                    return Ok(_fileUploadService.DeleteImagesFromFirebase(firebaseEntity));
                }*/
    }

    public class Test
    {
        public string ok { get; set; }
    }
}

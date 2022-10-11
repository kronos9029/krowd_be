using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static RevenueSharingInvest.Data.Helpers.FirestoreProvider;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/public/v1.0/POS")]
    [EnableCors]
    public class POSController : ControllerBase
    {

        private readonly FirestoreProvider _firestoreProvider;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public POSController(FirestoreProvider firestoreProvider, IUserService userService, IRoleService roleService)
        {
            _firestoreProvider = firestoreProvider;
            _userService = userService;
            _roleService = roleService;
        }

        [HttpPost]
        [Route("Krowd-upload")]
        public async Task<IActionResult> UploadBillsFromKrowd(List<BillEntity> billEntity)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            await _firestoreProvider.CreateBills(billEntity, currentUser.projectId);
            return Ok();
        }
        
        [HttpPost]
        [Route("Client-upload")]
        public async Task<IActionResult> UploadBillsFromPOS(List<BillEntity> billEntity, string projectId)
        {
            var result =  _firestoreProvider.CreateBills(billEntity, projectId);
            return Ok(result);
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetBillsDate(string projectId)
        {
            var result = await _firestoreProvider.GetDatesOfProject(projectId);
            return Ok(result);
        }
        //GET ALL
        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> GetTest(string projectId, string date)
        {
            var result = await _firestoreProvider.GetInvoiceDetailByDate(projectId, date);
            return Ok(result);
        }

    }
}

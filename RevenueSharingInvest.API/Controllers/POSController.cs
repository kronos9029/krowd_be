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

        public POSController(FirestoreProvider firestoreProvider)
        {
            _firestoreProvider = firestoreProvider;
        }

        [HttpPost]
        public async Task<IActionResult> ReciveBill(BillEntity billEntity)
        {
            await _firestoreProvider.CreateBills(billEntity);
            return Ok(0);
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

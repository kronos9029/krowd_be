using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/payments")]
    [EnableCors]
    //[Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public PaymentController(IPaymentService paymentService, IHttpContextAccessor httpContextAccessor)
        {
            _paymentService = paymentService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDTO paymentDTO)
        {
            var result = await _paymentService.CreatePayment(paymentDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = new List<PaymentDTO>();
            result = await _paymentService.GetAllPayments();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            PaymentDTO dto = new PaymentDTO();
            dto = await _paymentService.GetPaymentById(id);
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePayment([FromBody] PaymentDTO paymentDTO, [FromQuery] Guid paymentId)
        {
            var result = await _paymentService.UpdatePayment(paymentDTO, paymentId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePayment(Guid id)
        {
            var result = await _paymentService.DeletePaymentById(id);
            return Ok(result);
        }
    }
}

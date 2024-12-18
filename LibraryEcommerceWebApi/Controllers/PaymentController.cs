using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryEcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IControllerService<PaymentDto> _paymentService;

        public PaymentController(IControllerService<PaymentDto> paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("GetAllPayment"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pageNumber, int pageSize)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _paymentService.GetAllAsync(pageNumber, pageSize);
            return result.IsSuccess ? Ok(result.data) : BadRequest(result.Reason);
        }

        [HttpGet("GetPayment/{id}")]
        public async Task<IActionResult> GetPayment(string id)
        {
            var result = await _paymentService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result.data) : BadRequest(result.Reason);
        }

        [HttpPost("CreatePayment"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] PaymentDto paymentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _paymentService.CreateAsync(paymentDto);
            return result.IsSuccess ? Ok(result.data) : BadRequest(result.Reason);
        }

        [HttpPut("Update/{id}"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update(string id, [FromBody] PaymentDto paymentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _paymentService.UpdateAsync(id, paymentDto);
            return result.IsSuccess ? Ok(result.data) : BadRequest(result.Reason);
        }

        [HttpDelete("DeletePayment/{id}"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _paymentService.DeleteAsync(id);
            return result.IsSuccess ? Ok(result.data) : BadRequest(result.Reason);
        }
    }
}

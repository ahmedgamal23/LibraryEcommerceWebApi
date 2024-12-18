using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryEcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IControllerService<Discount> _controllerService;

        public DiscountController(IControllerService<Discount> controllerService)
        {
            _controllerService = controllerService;
        }

        [HttpGet("GetAllDiscounts"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pageNumber, int pageSize)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _controllerService.GetAllAsync(pageNumber, pageSize);
            if (result.IsSuccess)
                return Ok(result.data);
            return BadRequest(result.Reason);
        }

        [HttpGet, Route("GetDiscount"), Authorize(Roles = "Vendor")]
        public async Task<IActionResult> GetDiscount(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _controllerService.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result.data);
            return BadRequest(result.Reason);
        }

        [HttpPost, Route("Create"), Authorize(Roles = "Vendor")]
        public async Task<IActionResult> Create([FromBody] Discount discount)
        {
            if (!ModelState.IsValid)
                return BadRequest($"{ModelState}");

            var result = await _controllerService.CreateAsync(discount);
            if (result.IsSuccess)
                return Ok(result.data);
            return BadRequest(result.Reason);
        }

        [HttpPut, Route("Update/{id}"), Authorize(Roles = "Vendor")]
        public async Task<IActionResult> Update(string id, [FromBody] Discount discount)
        {
            if (!ModelState.IsValid)
                return BadRequest($"{ModelState}");
            var result = await _controllerService.UpdateAsync(id, discount);
            if (result.IsSuccess)
                return Ok(result.data);
            return BadRequest(result.Reason);
        }

        [HttpGet, Route("Delete"), Authorize(Roles = "Vendor")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.DeleteAsync(id);
            if (result.IsSuccess)
                return Ok(result.data);
            return BadRequest(result.Reason);
        }
    }
}

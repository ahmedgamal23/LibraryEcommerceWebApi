using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryEcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IControllerService<OrderDto> _controllerService;

        public OrderController(IControllerService<OrderDto> controllerService)
        {
            _controllerService = controllerService;
        }

        [HttpGet("GetAllOrder"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pageNumber, int pageSize)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.GetAllAsync(pageNumber, pageSize);
            return result.IsSuccess == true ?
                Ok(result.data) :
                BadRequest(result.Reason);
        }

        [HttpGet("GetOrder/{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrder(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.GetByIdAsync(id);
            return result.IsSuccess == true ?
                Ok(result.data) :
                BadRequest(result.Reason);
        }

        [HttpPost]
        [Route("CreateOrder"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _controllerService.CreateAsync(orderDto);
            return result.IsSuccess == true ?
                Ok(result.data) :
                BadRequest(result.Reason);
        }

        [HttpPut("Update/{id}"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update(string id, [FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.UpdateAsync(id, orderDto);
            return result.IsSuccess == true ?
                Ok(result.data) :
                BadRequest(result.Reason);
        }

        [HttpDelete("DeleteOrder/{id}"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.DeleteAsync(id);
            return result.IsSuccess == true ?
                Ok(result.data) :
                BadRequest(result.Reason);
        }
    }
}

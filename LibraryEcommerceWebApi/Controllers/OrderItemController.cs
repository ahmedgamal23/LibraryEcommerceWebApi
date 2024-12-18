using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryEcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IControllerService<OrderItemDto> _controllerService;

        public OrderItemController(IControllerService<OrderItemDto> controllerService)
        {
            _controllerService = controllerService;
        }

        [HttpGet("GetAllOrderItems"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index(int pageNumber, int pageSize)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.GetAllAsync(pageNumber, pageSize);
            return result.IsSuccess == true ?
                Ok(result.data) :
                BadRequest(result.Reason);
        }

        [HttpGet("GetOrderItem/{id}"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetOrderItem(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.GetByIdAsync(id);
            return result.IsSuccess == true ?
                Ok(result.data) :
                BadRequest(result.Reason);
        }

        [HttpPost]
        [Route("CreateOrderItem"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] OrderItemDto orderItemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _controllerService.CreateAsync(orderItemDto);
            return result.IsSuccess == true ?
                Ok(result.data) :
                BadRequest(result.Reason);
        }

        [HttpPut("Update/{id}"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update(string id, [FromBody] OrderItemDto orderItemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _controllerService.UpdateAsync(id, orderItemDto);
            return result.IsSuccess == true ?
                Ok(result.data) :
                BadRequest(result.Reason);
        }

        [HttpDelete("DeleteOrderItem/{id}"), Authorize(Roles = "Customer")]
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

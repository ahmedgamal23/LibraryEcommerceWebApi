using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryEcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IControllerService<ProductDto> _controllerService;

        public ProductController(IControllerService<ProductDto> controllerService)
        {
            _controllerService = controllerService;
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.GetAllAsync(pageNumber, pageSize);
            return result.IsSuccess ? Ok(result.data) : BadRequest(result.Reason);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result.data) : BadRequest(result.Reason);
        }

        [HttpPost, Authorize(Roles = "Vendor")]
        public async Task<IActionResult> Create([FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.CreateAsync(productDto);
            return result.IsSuccess ? Ok(result.data) : BadRequest(result.Reason);
        }

        [HttpPut("{id}"), Authorize(Roles = "Vendor")]
        public async Task<IActionResult> Update(string id, [FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.UpdateAsync(id, productDto);
            return result.IsSuccess ? Ok(result.data) : BadRequest(result.Reason);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Vendor")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.DeleteAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Reason);
        }
    }
}



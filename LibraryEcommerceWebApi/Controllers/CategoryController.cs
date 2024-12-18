using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryEcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IControllerService<CategoryDto> _controllerService;

        public CategoryController(IControllerService<CategoryDto> controllerService)
        {
            _controllerService = controllerService;
        }

        [HttpGet("GetAllCategories"), Authorize(Roles = "Customer")]
        public async Task<ActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _controllerService.GetAllAsync(pageNumber, pageSize);
            if (result.IsSuccess)
                return Ok(result.data);
            return BadRequest(result.Reason);
        }


        [HttpGet("GetCategory"), Authorize(Roles = "Customer")]
        public async Task<ActionResult> GetCategory(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _controllerService.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result.data);
            return BadRequest(result.Reason);
        }

        [HttpPost("AddNewCategory"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([FromForm] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _controllerService.CreateAsync(categoryDto);
            if (result.IsSuccess)
                return Ok(result.data);
            return BadRequest(result.Reason);
        }

        [HttpPut("UpdateCategory/{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(string id, [FromForm] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _controllerService.UpdateAsync(id, categoryDto);
            if (result.IsSuccess)
                return Ok(result.data);
            return BadRequest(result.Reason);
        }

        [HttpGet("Delete/{categoryId}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string categoryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _controllerService.DeleteAsync(categoryId);
            if (result.IsSuccess)
                return Ok(result.data);
            return BadRequest(result.Reason);
        }

    }
}

using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryEcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IControllerService<ReviewDto> _reviewService;

        public ReviewController(IControllerService<ReviewDto> reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet, Route("GetAllReviews"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            var response = await _reviewService.GetAllAsync(pageNumber, pageSize);
            return response.IsSuccess ? Ok(response.data) : BadRequest(response.Reason);
        }

        [HttpGet("{id}"), Authorize(Roles = "Vendor")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _reviewService.GetByIdAsync(id);
            return response.IsSuccess ? Ok(response.data) : BadRequest(response.Reason);
        }

        [HttpPost, Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] ReviewDto reviewDto)
        {
            var response = await _reviewService.CreateAsync(reviewDto);
            return response.IsSuccess ? Ok(response.data) : BadRequest(response.Reason);
        }

        [HttpPut("{id}"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update(string id, [FromBody] ReviewDto reviewDto)
        {
            var response = await _reviewService.UpdateAsync(id, reviewDto);
            return response.IsSuccess ? Ok(response.data) : BadRequest(response.Reason);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _reviewService.DeleteAsync(id);
            return response.IsSuccess ? Ok(response.data) : BadRequest(response);
        }
    }
}

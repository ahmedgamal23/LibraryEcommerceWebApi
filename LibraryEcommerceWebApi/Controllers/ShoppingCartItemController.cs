using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ShoppingCartItemController : ControllerBase
{
    private readonly IControllerService<ShoppingCartItemDto> _service;

    public ShoppingCartItemController(IControllerService<ShoppingCartItemDto> service)
    {
        _service = service;
    }

    [HttpGet, Route("GetAllCartItems"), Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
    {
        var response = await _service.GetAllAsync(pageNumber, pageSize);
        return response.IsSuccess ? Ok(response.data) : BadRequest(response);
    }

    [HttpGet("{id}"), Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetById(string id)
    {
        var response = await _service.GetByIdAsync(id);
        return response.IsSuccess ? Ok(response.data) : BadRequest(response);
    }

    [HttpPost, Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create([FromBody] ShoppingCartItemDto shoppingCartItemDto)
    {
        var response = await _service.CreateAsync(shoppingCartItemDto);
        return response.IsSuccess ? Ok(response.data) : BadRequest(response);
    }

    [HttpPut("{id}"), Authorize(Roles = "Customer")]
    public async Task<IActionResult> Update(string id, [FromBody] ShoppingCartItemDto shoppingCartItemDto)
    {
        var response = await _service.UpdateAsync(id, shoppingCartItemDto);
        return response.IsSuccess ? Ok(response.data) : BadRequest(response);
    }

    [HttpDelete("{id}"), Authorize(Roles = "Customer")]
    public async Task<IActionResult> Delete(string id)
    {
        var response = await _service.DeleteAsync(id);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}

using AutoMapper;
using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Shared;
using LibraryEcommerceWeb.Shared.DTOs;

public class ShoppingCartItemService : IControllerService<ShoppingCartItemDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ShoppingCartItemService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StateResponse> GetAllAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return new StateResponse { IsSuccess = false, Reason = "Invalid page number or size." };

        var allItems = await _unitOfWork.ShoppingCartItems.GetAllAsync();
        var pagedItems = allItems
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var total = allItems.Count();
        return new StateResponse
        {
            IsSuccess = true,
            data = new
            {
                TotalRecords = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)total / pageSize),
                Data = pagedItems
            }
        };
    }

    public async Task<StateResponse> GetByIdAsync(string id)
    {
        var item = await _unitOfWork.ShoppingCartItems.GetByIdAsync(id);
        return item != null
            ? new StateResponse { IsSuccess = true, data = item }
            : new StateResponse { IsSuccess = false, Reason = $"Item with ID {id} not found." };
    }

    public async Task<StateResponse> CreateAsync(ShoppingCartItemDto itemDto)
    {
        try
        {
            var item = _mapper.Map<ShoppingCartItem>(itemDto);
            await _unitOfWork.ShoppingCartItems.AddAsync(item);
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return new StateResponse { IsSuccess = true, data = item };

            return new StateResponse { IsSuccess = false, Reason = "Failed to add item to cart." };
        }
        catch (Exception ex)
        {
            return new StateResponse { IsSuccess = false, Reason = ex.Message };
        }
    }

    public async Task<StateResponse> UpdateAsync(string id, ShoppingCartItemDto itemDto)
    {
        var item = await _unitOfWork.ShoppingCartItems.GetByIdAsync(id);
        if (item == null)
            return new StateResponse { IsSuccess = false, Reason = $"Item with ID {id} not found." };

        _mapper.Map(itemDto, item);
        _unitOfWork.ShoppingCartItems.Update(item);

        return await _unitOfWork.SaveChangesAsync() > 0
            ? new StateResponse { IsSuccess = true, data = item }
            : new StateResponse { IsSuccess = false, Reason = "Failed to update item." };
    }

    public async Task<StateResponse> DeleteAsync(string id)
    {
        var item = await _unitOfWork.ShoppingCartItems.GetByIdAsync(id);
        if (item == null)
            return new StateResponse { IsSuccess = false, Reason = $"Item with ID {id} not found." };

        await _unitOfWork.ShoppingCartItems.DeleteAsync(id);

        return await _unitOfWork.SaveChangesAsync() > 0
            ? new StateResponse { IsSuccess = true }
            : new StateResponse { IsSuccess = false, Reason = "Failed to delete item." };
    }
}

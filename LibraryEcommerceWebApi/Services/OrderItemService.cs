using AutoMapper;
using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Shared;
using LibraryEcommerceWeb.Shared.DTOs;

namespace LibraryEcommerceWebApi.Services
{
    public class OrderItemService : IControllerService<OrderItemDto>
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public OrderItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StateResponse> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return new StateResponse { IsSuccess = false, Reason = "Page number and page size must be greater than 0." };

            var all = await _unitOfWork.OrderItems.GetAllAsync();

            if (all == null)
                return new StateResponse { IsSuccess = false, Reason = "Not Found" };
            var pagedOrderItems = all
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var total = all.Count();
            var result = new
            {
                TotalRecords = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)total / pageSize),
                Data = pagedOrderItems
            };
            return new StateResponse { IsSuccess = true, data = result };
        }

        public async Task<StateResponse> GetByIdAsync(string id)
        {
            var orderItems = await _unitOfWork.OrderItems.GetByIdAsync(id);
            return orderItems != null ?
                new StateResponse { IsSuccess = true, data = orderItems } :
                new StateResponse { IsSuccess = false, Reason = $"No orderItems found with this id {id}" };
        }

        public async Task<StateResponse> CreateAsync(OrderItemDto item)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(item.OrderId);
            if (order == null)
                return new StateResponse { IsSuccess = false, Reason = $"this order not found {item.OrderId}" };
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product == null)
                return new StateResponse { IsSuccess = false, Reason = $"this product not found {item.ProductId}" };

            var orderItem = _mapper.Map<OrderItem>(item);
            await _unitOfWork.OrderItems.AddAsync(orderItem);
            return await _unitOfWork.SaveChangesAsync() > 0 ?
                new StateResponse { IsSuccess = true, data = orderItem } :
                new StateResponse { IsSuccess = false, Reason = "Failed to add this order Item" };
        }

        public async Task<StateResponse> UpdateAsync(string id, OrderItemDto item)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(item.OrderId);
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (order == null)
                return new StateResponse { IsSuccess = false, Reason = $"this order not found {item.OrderId}" };
            if (product == null)
                return new StateResponse { IsSuccess = false, Reason = $"this product not found {item.ProductId}" };

            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (orderItem == null)
                return new StateResponse { IsSuccess = false, Reason = $"this id not found {id}" };
            orderItem.OrderId = item.OrderId;
            orderItem.ProductId = item.ProductId;
            orderItem.Quantity = item.Quantity;
            orderItem.UnitPrice = item.UnitPrice;

            _unitOfWork.OrderItems.Update(orderItem);
            return await _unitOfWork.SaveChangesAsync() > 0 ?
                new StateResponse { IsSuccess = true, data = orderItem } :
                new StateResponse { IsSuccess = false, Reason = $"Failed to update this order item id {id} " };
        }

        public async Task<StateResponse> DeleteAsync(string id)
        {
            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (orderItem == null)
                return new StateResponse { IsSuccess = false, Reason = $"this id not found {id}" };
            await _unitOfWork.OrderItems.DeleteAsync(id);
            return await _unitOfWork.SaveChangesAsync() > 0 ?
                new StateResponse { IsSuccess = true, data = orderItem, Reason = $"Delete successfully order Item for this id {id}" } :
                new StateResponse { IsSuccess = false, Reason = $"Failed to delete this order Item {id} " };
        }
    }
}



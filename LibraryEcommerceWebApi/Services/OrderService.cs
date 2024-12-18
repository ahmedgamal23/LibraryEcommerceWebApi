using AutoMapper;
using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Shared;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcommerceWebApi.Services
{
    public class OrderService : IControllerService<OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StateResponse> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return new StateResponse { IsSuccess = false, Reason = "Page number and page size must be greater than 0." };

            var all = await _unitOfWork.Orders.GetAllAsync(query => query.Include(c => c.OrderItems));

            if (all == null)
                return new StateResponse { IsSuccess = false, Reason = "Not Found" };
            var pagedOrders = all
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
                Data = pagedOrders
            };
            return new StateResponse { IsSuccess = true, data = result };
        }

        public async Task<StateResponse> GetByIdAsync(string id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return new StateResponse { IsSuccess = false, Reason = $"No order found with this id {id}" };
            return new StateResponse { IsSuccess = true, data = order };
        }

        public async Task<StateResponse> CreateAsync(OrderDto item)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(item.UserId);
            if (user == null)
                return new StateResponse { IsSuccess = false, Reason = $"this user not found {item.UserId}" };
            if (user.Role != "Customer")
                return new StateResponse { IsSuccess = false, Reason = $"this id {item.UserId} is not customer" };

            var order = _mapper.Map<Order>(item);
            await _unitOfWork.Orders.AddAsync(order);
            return await _unitOfWork.SaveChangesAsync() > 0 ?
                new StateResponse { IsSuccess = true, data = order } :
                new StateResponse { IsSuccess = false, Reason = "Failed to add this order" };
        }

        public async Task<StateResponse> UpdateAsync(string id, OrderDto item)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(item.UserId);
            if (user == null)
                return new StateResponse { IsSuccess = false, Reason = $"this user not found {item.UserId}" };
            if (user.Role != "Customer")
                return new StateResponse { IsSuccess = false, Reason = $"this id {item.UserId} is not customer" };

            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return new StateResponse { IsSuccess = false, Reason = $"this id not found {id}" };
            order.UserId = item.UserId;
            order.OrderDate = item.OrderDate;
            order.TotalAmount = item.TotalAmount;
            order.Status = item.Status;

            _unitOfWork.Orders.Update(order);
            return await _unitOfWork.SaveChangesAsync() > 0 ?
                new StateResponse { IsSuccess = true, data = order } :
                new StateResponse { IsSuccess = false, Reason = $"Failed to update this order {id} " };
        }

        public async Task<StateResponse> DeleteAsync(string id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return new StateResponse { IsSuccess = false, Reason = $"this id not found {id}" };
            await _unitOfWork.Orders.DeleteAsync(id);
            return await _unitOfWork.SaveChangesAsync() > 0 ?
                new StateResponse { IsSuccess = true, data = order } :
                new StateResponse { IsSuccess = false, Reason = $"Failed to delete this order {id} " };
        }
    }
}


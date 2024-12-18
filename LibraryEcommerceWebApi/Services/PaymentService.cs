using AutoMapper;
using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Shared;
using LibraryEcommerceWeb.Shared.DTOs;

namespace LibraryEcommerceWebApi.Services
{
    public class PaymentService : IControllerService<PaymentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StateResponse> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return new StateResponse { IsSuccess = false, Reason = "Page number and page size must be greater than 0." };

            var all = await _unitOfWork.Payments.GetAllAsync();
            if (all == null)
                return new StateResponse { IsSuccess = false, Reason = "No payments found." };

            var pagedPayments = all.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var total = all.Count();
            var result = new
            {
                TotalRecords = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)total / pageSize),
                Data = pagedPayments
            };

            return new StateResponse { IsSuccess = true, data = result };
        }

        public async Task<StateResponse> GetByIdAsync(string id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            return payment != null
                ? new StateResponse { IsSuccess = true, data = payment }
                : new StateResponse { IsSuccess = false, Reason = $"No payment found with id {id}" };
        }

        public async Task<StateResponse> CreateAsync(PaymentDto paymentDto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(paymentDto.OrderId);
            if (order == null)
                return new StateResponse { IsSuccess = false, Reason = $"Order not found with id {paymentDto.OrderId}" };

            var payment = _mapper.Map<Payment>(paymentDto);
            await _unitOfWork.Payments.AddAsync(payment);

            return await _unitOfWork.SaveChangesAsync() > 0
                ? new StateResponse { IsSuccess = true, data = payment }
                : new StateResponse { IsSuccess = false, Reason = "Failed to create payment." };
        }

        public async Task<StateResponse> UpdateAsync(string id, PaymentDto paymentDto)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                return new StateResponse { IsSuccess = false, Reason = $"Payment not found with id {id}" };

            var order = await _unitOfWork.Orders.GetByIdAsync(paymentDto.OrderId);
            if (order == null)
                return new StateResponse { IsSuccess = false, Reason = $"Order not found with id {paymentDto.OrderId}" };

            payment.OrderId = paymentDto.OrderId;
            payment.Amount = paymentDto.Amount;
            payment.PaymentStatus = paymentDto.PaymentStatus;

            _unitOfWork.Payments.Update(payment);
            return await _unitOfWork.SaveChangesAsync() > 0
                ? new StateResponse { IsSuccess = true, data = payment }
                : new StateResponse { IsSuccess = false, Reason = "Failed to update payment." };
        }

        public async Task<StateResponse> DeleteAsync(string id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                return new StateResponse { IsSuccess = false, Reason = $"Payment not found with id {id}" };

            await _unitOfWork.Payments.DeleteAsync(id);
            return await _unitOfWork.SaveChangesAsync() > 0
                ? new StateResponse { IsSuccess = true, data = payment, Reason = "Payment deleted successfully." }
                : new StateResponse { IsSuccess = false, Reason = "Failed to delete payment." };
        }
    }
}

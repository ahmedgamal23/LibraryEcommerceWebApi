using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Shared;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcommerceWebApi.Services
{
    public class DiscountService : IControllerService<Discount>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DiscountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StateResponse> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return new StateResponse { IsSuccess = false, Reason = "Page number and page size must be greater than 0." };

            var all = await _unitOfWork.Discounts.GetAllAsync(query => query.Include(c => c.Orders));

            if (all == null)
                return new StateResponse { IsSuccess = false, Reason = "No Discount exist." };

            var pagedDiscount = all
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
                Data = pagedDiscount
            };

            return new StateResponse { IsSuccess = true, data = result };
        }

        public async Task<StateResponse> GetByIdAsync(string id)
        {
            var discount = await _unitOfWork.Discounts.GetByIdAsync(id);
            if (discount != null)
                return new StateResponse { IsSuccess = true, data = discount };
            return new StateResponse { IsSuccess = false, Reason = $"this id {id} not exist" };
        }

        public async Task<StateResponse> CreateAsync(Discount item)
        {
            await _unitOfWork.Discounts.AddAsync(item);
            return await _unitOfWork.SaveChangesAsync() > 0 ?
                new StateResponse { IsSuccess = true, data = item, Reason = $"Add Successfully {item.Id}" } :
                new StateResponse { IsSuccess = false, Reason = "Can't saved this discount" };
        }

        public async Task<StateResponse> UpdateAsync(string id, Discount item)
        {
            var dbDiscount = await _unitOfWork.Discounts.GetByIdAsync(id);
            if (item == null)
                return new StateResponse { IsSuccess = false, Reason = $"this discount id {id} not exist" };
            dbDiscount!.Code = item.Code;
            dbDiscount.DiscountAmount = item.DiscountAmount;
            dbDiscount.ExpirationDate = item.ExpirationDate;
            _unitOfWork.Discounts.Update(dbDiscount);
            return await _unitOfWork.SaveChangesAsync() > 0 ?
                new StateResponse { IsSuccess = true, data = dbDiscount, Reason = $"Updated Successfully" } :
                new StateResponse { IsSuccess = false, Reason = $"Can't update this discount {id}" };
        }

        public async Task<StateResponse> DeleteAsync(string id)
        {
            await _unitOfWork.Discounts.DeleteAsync(id);
            return await _unitOfWork.SaveChangesAsync() > 0 ?
                new StateResponse { IsSuccess = true, Reason = $"Deleted successfully this discount {id}", data = id } :
                new StateResponse { IsSuccess = false, Reason = $"can't delete this account with this id {id}" };
        }
    }
}

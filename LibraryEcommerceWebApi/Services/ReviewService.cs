using AutoMapper;
using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Shared;
using LibraryEcommerceWeb.Shared.DTOs;

namespace LibraryEcommerceWebApi.Services
{
    public class ReviewService : IControllerService<ReviewDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StateResponse> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return new StateResponse { IsSuccess = false, Reason = "Page number and page size must be greater than 0." };

            var allReviews = await _unitOfWork.Reviews.GetAllAsync();

            if (allReviews == null)
                return new StateResponse { IsSuccess = false, Reason = "No reviews found." };

            var pagedReviews = allReviews
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var total = allReviews.Count();
            var result = new
            {
                TotalRecords = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)total / pageSize),
                Data = pagedReviews
            };

            return new StateResponse { IsSuccess = true, data = result };
        }

        public async Task<StateResponse> GetByIdAsync(string id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            return review != null
                ? new StateResponse { IsSuccess = true, data = review }
                : new StateResponse { IsSuccess = false, Reason = $"Review with ID {id} does not exist." };
        }

        public async Task<StateResponse> CreateAsync(ReviewDto reviewDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(reviewDto.ProductId);
            if (product == null)
                return new StateResponse { IsSuccess = false, Reason = $"Product with ID {reviewDto.ProductId} does not exist." };

            var user = await _unitOfWork.Users.GetByIdAsync(reviewDto.UserId);
            if (user == null)
                return new StateResponse { IsSuccess = false, Reason = $"User with ID {reviewDto.UserId} does not exist." };

            var review = _mapper.Map<Review>(reviewDto);
            await _unitOfWork.Reviews.AddAsync(review);

            return await _unitOfWork.SaveChangesAsync() > 0
                ? new StateResponse { IsSuccess = true, data = review }
                : new StateResponse { IsSuccess = false, Reason = "Error occurred while adding the review." };
        }

        public async Task<StateResponse> UpdateAsync(string id, ReviewDto reviewDto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null)
                return new StateResponse { IsSuccess = false, Reason = $"Review with ID {id} does not exist." };

            var product = await _unitOfWork.Products.GetByIdAsync(reviewDto.ProductId);
            if (product == null)
                return new StateResponse { IsSuccess = false, Reason = $"Product with ID {reviewDto.ProductId} does not exist." };

            var user = await _unitOfWork.Users.GetByIdAsync(reviewDto.UserId);
            if (user == null)
                return new StateResponse { IsSuccess = false, Reason = $"User with ID {reviewDto.UserId} does not exist." };

            review.Rating = reviewDto.Rating;
            review.Comment = reviewDto.Comment;
            review.CreatedAt = reviewDto.CreatedAt;
            review.ProductId = reviewDto.ProductId;
            review.UserId = reviewDto.UserId;

            _unitOfWork.Reviews.Update(review);

            return await _unitOfWork.SaveChangesAsync() > 0
                ? new StateResponse { IsSuccess = true, data = review }
                : new StateResponse { IsSuccess = false, Reason = $"Error occurred while updating the review with ID {id}." };
        }

        public async Task<StateResponse> DeleteAsync(string id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null)
                return new StateResponse { IsSuccess = false, Reason = $"Review with ID {id} does not exist." };

            await _unitOfWork.Reviews.DeleteAsync(id);
            return await _unitOfWork.SaveChangesAsync() > 0
                ? new StateResponse { IsSuccess = true, Reason = $"Review with ID {id} deleted successfully.", data = review }
                : new StateResponse { IsSuccess = false, Reason = $"Error occurred while deleting the review with ID {id}." };
        }
    }
}

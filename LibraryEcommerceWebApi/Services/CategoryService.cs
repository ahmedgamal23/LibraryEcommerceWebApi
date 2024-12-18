using AutoMapper;
using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Shared;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcommerceWebApi.Services
{
    public class CategoryService : IControllerService<CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public async Task<StateResponse> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return new StateResponse { IsSuccess = false, Reason = "Page number and page size must be greater than 0." };

            // Fetch all categories
            var categories = await _unitOfWork.Categories.GetAllAsync(query => query.Include(c => c.Products));

            // Apply pagination
            var pagedCategories = categories
                .Skip((pageNumber - 1) * pageSize) // Skip items for previous pages
                .Take(pageSize)                   // Take items for the current page
                .ToList();

            // Return result with paging info
            var totalRecords = categories.Count();
            var result = new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = pagedCategories
            };
            return new StateResponse { IsSuccess = true, data = result };
        }

        public async Task<StateResponse> GetByIdAsync(string id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category == null ?
                new StateResponse { IsSuccess = false, Reason = $"this id {id} not exist", data = category } :
                new StateResponse { IsSuccess = true, data = category };
        }

        public async Task<StateResponse> CreateAsync(CategoryDto categoryDto)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var categoryExist = categories.Where(c => c.Name == categoryDto.Name).FirstOrDefault();
            if (categoryExist != null)
                return new StateResponse { IsSuccess = false, Reason = $"Aleardy Exist {categoryDto.Name}" };

            var category = _mapper.Map<Category>(categoryDto);

            if (categoryDto.FormFile != null)
            {
                string imageName = $"{Guid.NewGuid()}{Path.GetExtension(categoryDto.FormFile!.FileName)}";
                string imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images");

                if (!Directory.Exists(imageDirectory))
                    Directory.CreateDirectory(imageDirectory);

                string fullPath = Path.Combine(imageDirectory, imageName);
                await using (var stream = new FileStream(fullPath, FileMode.Create))
                    await categoryDto.FormFile.CopyToAsync(stream);

                category.ImagePath = Path.Combine("assets", "images", imageName);
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return new StateResponse { IsSuccess = true, data = category };
        }

        public async Task<StateResponse> UpdateAsync(string id, CategoryDto categoryDto)
        {
            // Fetch category by Id from the URL path
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return new StateResponse { IsSuccess = false, Reason = "Category not found." };
            }

            // Handle file upload for category image
            if (categoryDto.FormFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(categoryDto.FormFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new StateResponse { IsSuccess = false, Reason = "Invalid file type. Only images are allowed." };
                }

                if (categoryDto.FormFile.Length > 2 * 1024 * 1024) // 2MB limit
                {
                    return new StateResponse { IsSuccess = false, Reason = "File size exceeds 2MB limit." };
                }

                var sanitizedFileName = Path.GetFileName(categoryDto.FormFile.FileName);
                var imageName = $"{Guid.NewGuid()}_{sanitizedFileName}";
                var imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images");

                if (!Directory.Exists(imageDirectory))
                    Directory.CreateDirectory(imageDirectory);

                string fullPath = Path.Combine(imageDirectory, imageName);
                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await categoryDto.FormFile.CopyToAsync(stream);
                }

                // Remove existing image if exists
                var existingImageFullPath = Path.Combine(imageDirectory, category.ImagePath);
                if (Directory.Exists(existingImageFullPath))
                {
                    Directory.Delete(existingImageFullPath);
                }

                category.ImagePath = Path.Combine("assets", "images", imageName);
            }

            // Update the category name
            category.Name = categoryDto.Name;
            _unitOfWork.Categories.Update(category);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return new StateResponse { IsSuccess = true, data = category };
            }
            catch (Exception ex)
            {
                return new StateResponse { IsSuccess = false, Reason = $"An error occurred while updating the category.\n {ex}" };
            }
        }

        public async Task<StateResponse> DeleteAsync(string id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return new StateResponse { IsSuccess = false, Reason = "this is Id not exist" };

            if (Directory.Exists(category.ImagePath))
                Directory.Delete(category.ImagePath);

            await _unitOfWork.Categories.DeleteAsync(id);
            return await _unitOfWork.SaveChangesAsync() > 0 ?
                new StateResponse { IsSuccess = true, data = category } :
                new StateResponse { IsSuccess = false, Reason = "Error while deleting this category" };
        }
    }
}



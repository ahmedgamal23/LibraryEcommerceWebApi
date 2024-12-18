using AutoMapper;
using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Shared;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcommerceWebApi.Services
{
    public class ProductService : IControllerService<ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<StateResponse> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return new StateResponse { IsSuccess = false, Reason = "Page number and page size must be greater than 0." };

            var allProducts = await _unitOfWork.Products.GetAllAsync();
            if (allProducts == null)
                return new StateResponse { IsSuccess = false, Reason = "No products found." };

            var pagedProducts = allProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new
            {
                TotalRecords = allProducts.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)allProducts.Count() / pageSize),
                Data = pagedProducts
            };

            return new StateResponse { IsSuccess = true, data = result };
        }

        public async Task<StateResponse> GetByIdAsync(string id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            return product != null
                ? new StateResponse { IsSuccess = true, data = product }
                : new StateResponse { IsSuccess = false, Reason = $"Product with ID {id} not found." };
        }

        public async Task<StateResponse> CreateAsync(ProductDto productDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(productDto.VendorId);
            if (user == null || user.Role != "Vendor")
                return new StateResponse { IsSuccess = false, Reason = "Invalid vendor ID." };

            if (productDto.FormFile == null)
                return new StateResponse { IsSuccess = false, Reason = "Product image is required." };

            // Save product image
            string imageName = $"{Guid.NewGuid()}{Path.GetExtension(productDto.FormFile.FileName)}";
            string directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string fullPath = Path.Combine(directoryPath, imageName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await productDto.FormFile.CopyToAsync(stream);
            }

            var product = _mapper.Map<Product>(productDto);
            product.ImagePath = Path.Combine("assets", "images", imageName);

            await _unitOfWork.Products.AddAsync(product);
            return await _unitOfWork.SaveChangesAsync() > 0
                ? new StateResponse { IsSuccess = true, data = product }
                : new StateResponse { IsSuccess = false, Reason = "Failed to create product." };
        }

        public async Task<StateResponse> UpdateAsync(string id, ProductDto productDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return new StateResponse { IsSuccess = false, Reason = $"Product with ID {id} not found." };

            var user = await _unitOfWork.Users.GetByIdAsync(productDto.VendorId);
            if (user == null || user.Role != "Vendor")
                return new StateResponse { IsSuccess = false, Reason = "Invalid vendor ID." };

            if (productDto.FormFile != null)
            {
                // Save new image
                string imageName = $"{Guid.NewGuid()}{Path.GetExtension(productDto.FormFile.FileName)}";
                string directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images");

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                string fullPath = Path.Combine(directoryPath, imageName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await productDto.FormFile.CopyToAsync(stream);
                }

                // Delete old image
                if (File.Exists(product.ImagePath))
                    File.Delete(product.ImagePath);

                product.ImagePath = Path.Combine("assets", "images", imageName);
            }

            _mapper.Map(productDto, product);
            _unitOfWork.Products.Update(product);

            return await _unitOfWork.SaveChangesAsync() > 0
                ? new StateResponse { IsSuccess = true, data = product }
                : new StateResponse { IsSuccess = false, Reason = "Failed to update product." };
        }

        public async Task<StateResponse> DeleteAsync(string id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return new StateResponse { IsSuccess = false, Reason = $"Product with ID {id} not found." };

            // Delete product image
            if (File.Exists(product.ImagePath))
                File.Delete(product.ImagePath);

            await _unitOfWork.Products.DeleteAsync(id);

            return await _unitOfWork.SaveChangesAsync() > 0
                ? new StateResponse { IsSuccess = true, Reason = $"Product {id} deleted successfully.", data = product }
                : new StateResponse { IsSuccess = false, Reason = "Failed to delete product." };
        }
    }
}

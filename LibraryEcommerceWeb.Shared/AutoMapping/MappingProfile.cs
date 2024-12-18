using AutoMapper;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Shared.DTOs;

namespace LibraryEcommerceWeb.Shared.AutoMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map User to RegisterDto
            CreateMap<User, RegisterDto>();
            CreateMap<RegisterDto, User>();

            // Map User to LoginDto
            CreateMap<User, LoginDto>();
            CreateMap<LoginDto, User>();

            // Map Category to CategoryDto
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            // Map Product to ProductDto
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();

            // Map Order to OrderDto
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();

            // Map OrderItem to OrderItemDto
            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<OrderItemDto, OrderItem>();

            // Map Payment to PaymentDto
            CreateMap<Payment, PaymentDto>();
            CreateMap<PaymentDto, Payment>();

            // Map ShoppingCartItem to ShoppingCartItemDto
            CreateMap<ShoppingCartItem, ShoppingCartItemDto>();
            CreateMap<ShoppingCartItemDto, ShoppingCartItem>();

            // Map Review to ReviewDto
            CreateMap<Review, ReviewDto>();
            CreateMap<ReviewDto, Review>();

        }
    }
}

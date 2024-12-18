
using LibraryEcommerceWeb.Application.Interfaces;
using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Infrastructure.Data;
using LibraryEcommerceWeb.Infrastructure.Repositories;
using LibraryEcommerceWeb.Shared.AutoMapping;
using LibraryEcommerceWeb.Shared.DTOs;
using LibraryEcommerceWebApi.Extensions;
using LibraryEcommerceWebApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcommerceWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });

            // Add AppDbContext with connection string 
            builder.Services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("LibraryEcommerceConnectionString"));
            });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // add jwt authentication extention method
            builder.Services.AddCustomJwtAuth(builder.Configuration);

            // register AutoMapper
            //builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            // register password hasher
            builder.Services.AddScoped<PasswordHasher<User>>();
            // register IUnitOfWork , UnitOfWork
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            // register IUserRepository , UserRepository
            builder.Services.AddScoped<IUserRepository<User>, UserRepository>();
            // register Generated token
            builder.Services.AddScoped<GenerateToken>();
            // register IEmailSender, EmailSender
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            // register IControllerService, CategoryService
            builder.Services.AddScoped<IControllerService<CategoryDto>, CategoryService>();
            // register IControllerService, DiscountService
            builder.Services.AddScoped<IControllerService<Discount>, DiscountService>();
            // register IControllerService, OrderItemsService
            builder.Services.AddScoped<IControllerService<OrderItemDto>, OrderItemService>();
            // register IControllerService, OrderService
            builder.Services.AddScoped<IControllerService<OrderDto>, OrderService>();
            // register IControllerService, PaymentService
            builder.Services.AddScoped<IControllerService<PaymentDto>, PaymentService>();
            // register IControllerService, ProductService
            builder.Services.AddScoped<IControllerService<ProductDto>, ProductService>();
            // register IControllerService, ReviewService
            builder.Services.AddScoped<IControllerService<ReviewDto>, ReviewService>();
            // register IControllerService, ShoppingCartItemService
            builder.Services.AddScoped<IControllerService<ShoppingCartItemDto>, ShoppingCartItemService>();
            // register ISmtpClient , SmtpClientWrapper
            builder.Services.AddScoped<ISmtpClient, SmtpClientWrapper>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.Render(); // extension method to change (401 Response) to (Not Authorize)

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

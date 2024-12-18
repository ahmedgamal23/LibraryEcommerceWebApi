using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Infrastructure.Data;
using LibraryEcommerceWeb.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryEcommerceWeb.Tests.InfrastructureTests
{
    [TestFixture]
    public class GenericProductRepository_Test
    {
        #region Product Generic Repository Functions Test

        private AppDbContext _context;
        private GenericRepository<Product> _repository;

        [SetUp]
        public void Setup()
        {
            // use in-memory-database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryEcommerceWebApi")
                .Options;

            // initialize AppDbContext
            _context = new AppDbContext(options);

            // Seed the database with test data
            _context.Set<Product>().AddRange(new List<Product>
            {
                new Product { Id = "1", Name = "Product 1", Description = "Test product 1", Price = 100.00m, StockQuantity = 10, IsDigital = false, CategoryId = "1", VendorId = "1", ImagePath = "product1.jpg" },
                new Product { Id = "2", Name = "Product 2", Description = "Test product 2", Price = 50.00m, StockQuantity = 20, IsDigital = true, DigitalUrl = "https://example.com/product2", CategoryId = "1", VendorId = "2", ImagePath = "product2.jpg" },
                new Product { Id = "3", Name = "Product 3", Description = "Test product 3", Price = 150.00m, StockQuantity = 5, IsDigital = false, CategoryId = "2", VendorId = "1", ImagePath = "product3.jpg" }
            });
            _context.SaveChanges();

            // initialize repository
            _repository = new GenericRepository<Product>(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // to clean in-memory-database after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region Get All Async Function Test
        [Test]
        public async Task GetAllAsync_Test_Return_Count_Of_All_Products()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllAsync_Test_Return_Valid_Products()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(r => r.Name == "Product 1"), Is.True);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_Invalid_Products()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(r => r.Name == "NonExistentProduct"), Is.False);
        }

        #endregion

        #region Get By Id Async Function Test
        [Test]
        public async Task GetByIdAsync_Test_Return_Valid_Product()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.Name, Is.EqualTo("Product 1"));
        }

        [Test]
        public async Task GetByIdAsync_Test_Return_Invalid_Product()
        {
            // Act
            var result = await _repository.GetByIdAsync("99");
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByIdAsync_Test_Set_Empty_Id()
        {
            // Act & Assert
            Assert.ThatAsync(async () => { await _repository.GetByIdAsync(""); }, Is.Null, "Id can't be null");
        }

        #endregion

        #region Add Async Function Test
        [Test]
        public async Task AddAsync_Test_Add_Valid_Product()
        {
            // Arrange
            var product = new Product { Id = "4", Name = "Product 4", Description = "Test product 4", Price = 200.00m, StockQuantity = 30, IsDigital = false, CategoryId = "2", VendorId = "2", ImagePath = "product4.jpg" };

            // Act
            await _repository.AddAsync(product);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<Product>().FindAsync("4");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Product 4"));
        }

        [Test]
        public void AddAsync_Test_Add_Null_Product_Throws_Exception()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _repository.AddAsync(null!);
                await _context.SaveChangesAsync();
            });
        }

        [Test]
        public async Task AddAsync_Test_Add_Duplicate_Product()
        {
            // Arrange
            var product1 = new Product { Id = "5", Name = "Product 5", Description = "Test product 5", Price = 300.00m, StockQuantity = 50, IsDigital = true, DigitalUrl = "https://example.com/product5", CategoryId = "1", VendorId = "1", ImagePath = "product5.jpg" };
            var product2 = new Product { Id = "5", Name = "Product 6", Description = "Test product 6", Price = 250.00m, StockQuantity = 40, IsDigital = false, CategoryId = "2", VendorId = "2", ImagePath = "product6.jpg" };

            await _repository.AddAsync(product1);
            await _context.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _repository.AddAsync(product2);
                await _context.SaveChangesAsync();
            });
        }

        #endregion

        #region Update Async Function Test
        [Test]
        public async Task UpdateAsync_Test_Update_Valid_Product()
        {
            // Arrange
            var existingProduct = await _repository.GetByIdAsync("3");
            existingProduct!.Price = 175.00m;

            // Act
            _repository.Update(existingProduct);
            await _context.SaveChangesAsync();

            // Assert
            var updatedProduct = await _context.Set<Product>().FindAsync("3");
            Assert.That(updatedProduct, Is.Not.Null);
            Assert.That(updatedProduct!.Price, Is.EqualTo(175.00m));
        }

        [Test]
        public void UpdateAsync_Test_Update_Null_Product_Throws_Exception()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _repository.Update(null!);
            });
        }

        [Test]
        public void UpdateAsync_Test_Update_NonExistent_Product()
        {
            // Arrange
            var nonExistentProduct = new Product { Id = "99", Name = "NonExistentProduct", Description = "Non existent", Price = 100.00m, StockQuantity = 0, IsDigital = false, CategoryId = "1", VendorId = "1", ImagePath = "nonexistent.jpg" };

            // Act & Assert
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                _repository.Update(nonExistentProduct);
                await _context.SaveChangesAsync();
            });
        }

        #endregion

        #region Delete Async Function Test
        [Test]
        public async Task DeleteAsync_Test_Delete_Existing_Product_Success()
        {
            // Arrange
            var productToDelete = await _repository.GetByIdAsync("3");

            // Act
            await _repository.DeleteAsync("3");
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<Product>().FindAsync("3");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteAsync_Test_Delete_NonExistent_Product_No_Exception()
        {
            // Act
            Assert.DoesNotThrowAsync(async () =>
            {
                await _repository.DeleteAsync("99");
                await _context.SaveChangesAsync();
            });
        }

        [Test]
        public void DeleteAsync_Test_Delete_With_Null_Or_Empty_Id_No_Exception()
        {
            // Act
            Assert.DoesNotThrowAsync(async () =>
            {
                await _repository.DeleteAsync(string.Empty);
                await _repository.DeleteAsync(null!);
                await _context.SaveChangesAsync();
            });
        }

        #endregion

        #endregion
    }
}

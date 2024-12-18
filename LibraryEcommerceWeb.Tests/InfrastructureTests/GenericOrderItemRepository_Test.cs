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
    public class GenericOrderItemRepository_Test
    {
        #region OrderItem Generic Repository Functions Test

        private AppDbContext _context;
        private GenericRepository<OrderItem> _repository;

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
            _context.Set<OrderItem>().AddRange(new List<OrderItem>
            {
                new OrderItem { Id = "1", ProductId = "prod1", OrderId = "order1", Quantity = 2, UnitPrice = 50.00m },
                new OrderItem { Id = "2", ProductId = "prod2", OrderId = "order2", Quantity = 1, UnitPrice = 100.00m },
                new OrderItem { Id = "3", ProductId = "prod3", OrderId = "order3", Quantity = 3, UnitPrice = 75.00m },
                new OrderItem { Id = "4", ProductId = "prod4", OrderId = "order4", Quantity = 5, UnitPrice = 25.00m }
            });
            _context.SaveChanges();

            // initialize repository
            _repository = new GenericRepository<OrderItem>(_context);
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
        public async Task GetAllAsync_Test_Return_Count_Of_All_Data()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Count(), Is.EqualTo(4));
        }

        [Test]
        public async Task GetAllAsync_Test_Return_Valid_Data()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(r => r.Quantity == 2), Is.True);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_Invalid_Data()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(r => r.Quantity == 100), Is.False);
        }

        #endregion

        #region Get By Id Async Function Test
        [Test]
        public async Task GetByIdAsync_Test_Return_Valid_Data()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.Quantity, Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_Test_Return_Invalid_Data()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.UnitPrice, Is.Not.EqualTo(100.00m));
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
        public async Task AddAsync_Test_Add_Valid_Item()
        {
            // Arrange
            var item = new OrderItem { Id = "5", ProductId = "prod5", OrderId = "order5", Quantity = 3, UnitPrice = 40.00m };

            // Act
            await _repository.AddAsync(item);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<OrderItem>().FindAsync("5");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Quantity, Is.EqualTo(3));
        }

        [Test]
        public void AddAsync_Test_Add_Null_Item_Throws_Exception()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _repository.AddAsync(null!);
                await _context.SaveChangesAsync();
            });
        }

        [Test]
        public async Task AddAsync_Test_Add_Duplicate_Item()
        {
            // Arrange
            var item1 = new OrderItem { Id = "6", ProductId = "prod6", OrderId = "order6", Quantity = 1, UnitPrice = 150.00m };
            var item2 = new OrderItem { Id = "6", ProductId = "prod7", OrderId = "order7", Quantity = 2, UnitPrice = 75.00m };

            await _repository.AddAsync(item1);
            await _context.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _repository.AddAsync(item2);
                await _context.SaveChangesAsync();
            });
        }

        #endregion

        #region Update Async Function Test
        [Test]
        public async Task UpdateAsync_Test_Update_Valid_Item()
        {
            // Arrange
            var existingItem = await _repository.GetByIdAsync("4");
            existingItem!.Quantity = 10;

            // Act
            _repository.Update(existingItem);
            await _context.SaveChangesAsync();

            // Assert
            var updatedItem = await _context.Set<OrderItem>().FindAsync("4");
            Assert.That(updatedItem, Is.Not.Null);
            Assert.That(updatedItem!.Quantity, Is.EqualTo(10));
        }

        [Test]
        public void UpdateAsync_Test_Update_Null_Item_Throws_Exception()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _repository.Update(null!);
            });
        }

        [Test]
        public void UpdateAsync_Test_Update_NonExistent_Item()
        {
            // Arrange
            var nonExistentItem = new OrderItem { Id = "99", ProductId = "prod99", OrderId = "order99", Quantity = 5, UnitPrice = 20.00m };

            // Act & Assert
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                _repository.Update(nonExistentItem);
                await _context.SaveChangesAsync();
            });
        }

        #endregion

        #region Delete Async Function Test
        [Test]
        public async Task DeleteAsync_Test_Delete_Existing_Item_Success()
        {
            // Arrange
            var itemToDelete = await _repository.GetByIdAsync("4");

            // Act
            await _repository.DeleteAsync("4");
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<OrderItem>().FindAsync("4");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteAsync_Test_Delete_NonExistent_Item_No_Exception()
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

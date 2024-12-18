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
    public class GenericOrderRepository_Test
    {
        #region Order Generic Repository Functions Test

        private AppDbContext _context;
        private GenericRepository<Order> _repository;

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
            _context.Set<Order>().AddRange(new List<Order>
            {
                new Order { Id = "1", TotalAmount = 100, Status = "Pending", OrderDate = DateTime.Now.AddDays(-1), UserId = "user1" },
                new Order { Id = "2", TotalAmount = 150, Status = "Processing", OrderDate = DateTime.Now.AddDays(-2), UserId = "user2" },
                new Order { Id = "3", TotalAmount = 200, Status = "Completed", OrderDate = DateTime.Now.AddDays(-3), UserId = "user3" },
                new Order { Id = "4", TotalAmount = 250, Status = "Canceled", OrderDate = DateTime.Now.AddDays(-4), UserId = "user4" }
            });
            _context.SaveChanges();

            // initialize repository
            _repository = new GenericRepository<Order>(_context);
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
            Assert.That(result.Any(r => r.Status == "Pending"), Is.True);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_InValid_Data()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(r => r.Status == "Shipped"), Is.False);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_All_Data_Success()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            var expectedStatuses = new[] { "Pending", "Processing", "Completed", "Canceled" };
            Assert.That(result.Select(r => r.Status), Is.EquivalentTo(expectedStatuses));
        }

        #endregion

        #region Get By Id Async Function Test
        [Test]
        public async Task GetByIdAsync_Test_Return_Valid_Data()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.TotalAmount, Is.EqualTo(100));
        }

        [Test]
        public async Task GetByIdAsync_Test_Return_InValid_Data()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.Status, Is.Not.EqualTo("Completed"));
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
            var item = new Order { Id = "5", TotalAmount = 300, Status = "Pending", OrderDate = DateTime.Now, UserId = "user5" };

            // Act
            await _repository.AddAsync(item);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<Order>().FindAsync("5");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TotalAmount, Is.EqualTo(300));
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
            var item1 = new Order { Id = "6", TotalAmount = 100, Status = "Pending", OrderDate = DateTime.Now, UserId = "user6" };
            var item2 = new Order { Id = "6", TotalAmount = 150, Status = "Processing", OrderDate = DateTime.Now, UserId = "user7" };

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
            existingItem!.Status = "Completed";

            // Act
            _repository.Update(existingItem);
            await _context.SaveChangesAsync();

            // Assert
            var updatedItem = await _context.Set<Order>().FindAsync("4");
            Assert.That(updatedItem, Is.Not.Null);
            Assert.That(updatedItem!.Status, Is.EqualTo("Completed"));
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
            var nonExistentItem = new Order { Id = "99", TotalAmount = 0, Status = "Pending", OrderDate = DateTime.Now, UserId = "user99" };

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
            var result = await _context.Set<Order>().FindAsync("4");
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

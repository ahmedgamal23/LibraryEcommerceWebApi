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
    public class GenericDiscountRepository_Test
    {
        #region Discount Generic Repository Functions Test

        private AppDbContext _context;
        private GenericRepository<Discount> _repository;

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
            _context.Set<Discount>().AddRange(new List<Discount>
            {
                new Discount { Id = "1", Code = "DISC1", DiscountAmount = 10.5m, ExpirationDate = DateTime.Now.AddDays(10) },
                new Discount { Id = "2", Code = "DISC2", DiscountAmount = 15.0m, ExpirationDate = DateTime.Now.AddDays(20) },
                new Discount { Id = "3", Code = "DISC3", DiscountAmount = 20.0m, ExpirationDate = DateTime.Now.AddDays(30) },
                new Discount { Id = "4", Code = "DISC4", DiscountAmount = 25.0m, ExpirationDate = DateTime.Now.AddDays(40) }
            });
            _context.SaveChanges();

            // initialize repository
            _repository = new GenericRepository<Discount>(_context);
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
            Assert.That(result.Any(r => r.Code == "DISC1"), Is.True);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_InValid_Data()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(r => r.Code == "DISC5"), Is.False);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_All_Data_Success()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            var expectedCodes = new[] { "DISC1", "DISC2", "DISC3", "DISC4" };
            Assert.That(result.Select(r => r.Code), Is.EquivalentTo(expectedCodes));
        }

        #endregion

        #region Get By Id Async Function Test
        [Test]
        public async Task GetByIdAsync_Test_Return_Valid_Data()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.Code, Is.EqualTo("DISC1"));
        }

        [Test]
        public async Task GetByIdAsync_Test_Return_InValid_Data()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.Code, Is.Not.EqualTo("DISC2"));
        }

        [Test]
        public void GetByIdAsync_Test_Set_Empty_Id()
        {
            // Act & Assert
            Assert.ThatAsync(async () => { await _repository.GetByIdAsync(""); }, Is.Null, "Id Can't be null");
        }

        #endregion

        #region Add Async Function Test

        [Test]
        public async Task AddAsync_Test_Add_Valid_Item()
        {
            // Arrange
            var item = new Discount { Id = "5", Code = "DISC5", DiscountAmount = 30.0m, ExpirationDate = DateTime.Now.AddDays(50) };

            // Act
            await _repository.AddAsync(item);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<Discount>().FindAsync("5");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Code, Is.EqualTo("DISC5"));
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
            var item1 = new Discount { Id = "6", Code = "DISC6", DiscountAmount = 10.0m, ExpirationDate = DateTime.Now.AddDays(60) };
            var item2 = new Discount { Id = "6", Code = "DISC6", DiscountAmount = 10.0m, ExpirationDate = DateTime.Now.AddDays(60) };

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

            existingItem!.DiscountAmount = 30.0m;

            // Act
            _repository.Update(existingItem);
            await _context.SaveChangesAsync();

            // Assert
            var updatedItem = await _context.Set<Discount>().FindAsync("4");
            Assert.That(updatedItem, Is.Not.Null);
            Assert.That(updatedItem!.DiscountAmount, Is.EqualTo(30.0m));
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
            var nonExistentItem = new Discount { Id = "99", Code = "DISC99", DiscountAmount = 50.0m, ExpirationDate = DateTime.Now.AddDays(70) };

            // Act & Assert
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                _repository.Update(nonExistentItem);
                await _context.SaveChangesAsync();
            });
        }

        [Test]
        public async Task UpdateAsync_Test_Update_Item_Without_Changes()
        {
            // Arrange
            var existingItem = new Discount { Id = "5", Code = "DISC5", DiscountAmount = 30.0m, ExpirationDate = DateTime.Now.AddDays(50) };
            await _context.Set<Discount>().AddAsync(existingItem);
            await _context.SaveChangesAsync();

            // Act
            _repository.Update(existingItem);
            await _context.SaveChangesAsync();

            // Assert
            var unchangedItem = await _context.Set<Discount>().FindAsync("5");
            Assert.That(unchangedItem, Is.Not.Null);
            Assert.That(unchangedItem!.DiscountAmount, Is.EqualTo(30.0m));
        }

        #endregion

        #region Delete Async Function Test
        [Test]
        public async Task DeleteAsync_Test_Delete_Existing_Item_Success()
        {
            // Arrange
            var itemToDelete = _repository.GetByIdAsync("4");

            // Act
            await _repository.DeleteAsync("4");
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<Discount>().FindAsync("4");
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

using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Infrastructure.Data;
using LibraryEcommerceWeb.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryEcommerceWeb.Tests.InfrastructureTests
{
    [TestFixture]
    public class GenericShoppingCartItemRepository_Test
    {
        #region ShoppingCartItem Generic Repository Functions Test

        private AppDbContext _context;
        private GenericRepository<ShoppingCartItem> _repository;

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
            _context.Set<ShoppingCartItem>().AddRange(new List<ShoppingCartItem>
            {
                new ShoppingCartItem { Id = "1", ProductId = "1", UserId = "1", Quantity = 2 },
                new ShoppingCartItem { Id = "2", ProductId = "2", UserId = "2", Quantity = 1 },
                new ShoppingCartItem { Id = "3", ProductId = "3", UserId = "1", Quantity = 5 }
            });
            _context.SaveChanges();

            // initialize repository
            _repository = new GenericRepository<ShoppingCartItem>(_context);
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
        public async Task GetAllAsync_Test_Return_Count_Of_All_ShoppingCartItems()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllAsync_Test_Return_Valid_ShoppingCartItems()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(item => item.Quantity == 2), Is.True);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_Invalid_ShoppingCartItems()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(item => item.Quantity == 6), Is.False);
        }

        #endregion

        #region Get By Id Async Function Test
        [Test]
        public async Task GetByIdAsync_Test_Return_Valid_ShoppingCartItem()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.Quantity, Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_Test_Return_Invalid_ShoppingCartItem()
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
        public async Task AddAsync_Test_Add_Valid_ShoppingCartItem()
        {
            // Arrange
            var shoppingCartItem = new ShoppingCartItem { Id = "4", ProductId = "2", UserId = "3", Quantity = 1 };

            // Act
            await _repository.AddAsync(shoppingCartItem);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<ShoppingCartItem>().FindAsync("4");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void AddAsync_Test_Add_Null_ShoppingCartItem_Throws_Exception()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _repository.AddAsync(null!);
                await _context.SaveChangesAsync();
            });
        }

        [Test]
        public void AddAsync_Test_Add_Invalid_Quantity_Throws_Exception()
        {
            // Arrange
            var shoppingCartItem = new ShoppingCartItem { Id = "5", ProductId = "1", UserId = "2", Quantity = 0 };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
            {
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(shoppingCartItem, new ValidationContext(shoppingCartItem), validationResults, true);
                if (!isValid)
                {
                    throw new ValidationException(validationResults.FirstOrDefault()?.ErrorMessage);
                }

                // If validation passes, attempt to add and save
                _repository.AddAsync(shoppingCartItem).Wait();
                _context.SaveChangesAsync().Wait();
            });

            Assert.That(exception?.Message, Is.EqualTo("The field Quantity must be greater than 0."));
        }


        #endregion

        #region Update Async Function Test
        [Test]
        public async Task UpdateAsync_Test_Update_Valid_ShoppingCartItem()
        {
            // Arrange
            var existingCartItem = await _repository.GetByIdAsync("2");
            existingCartItem!.Quantity = 3;

            // Act
            _repository.Update(existingCartItem);
            await _context.SaveChangesAsync();

            // Assert
            var updatedItem = await _context.Set<ShoppingCartItem>().FindAsync("2");
            Assert.That(updatedItem, Is.Not.Null);
            Assert.That(updatedItem!.Quantity, Is.EqualTo(3));
        }

        [Test]
        public void UpdateAsync_Test_Update_Null_ShoppingCartItem_Throws_Exception()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _repository.Update(null!);
            });
        }

        [Test]
        public void UpdateAsync_Test_Update_NonExistent_ShoppingCartItem()
        {
            // Arrange
            var nonExistentItem = new ShoppingCartItem { Id = "99", ProductId = "1", UserId = "3", Quantity = 2 };

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
        public async Task DeleteAsync_Test_Delete_Existing_ShoppingCartItem_Success()
        {
            // Arrange
            var itemToDelete = await _repository.GetByIdAsync("3");

            // Act
            await _repository.DeleteAsync("3");
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<ShoppingCartItem>().FindAsync("3");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteAsync_Test_Delete_NonExistent_ShoppingCartItem_No_Exception()
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

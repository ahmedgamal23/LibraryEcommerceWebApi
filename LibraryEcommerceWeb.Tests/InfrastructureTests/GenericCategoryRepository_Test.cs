using LibraryEcommerceWeb.Domain.Entities;
using LibraryEcommerceWeb.Infrastructure.Data;
using LibraryEcommerceWeb.Infrastructure.Repositories;
using LibraryEcommerceWeb.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryEcommerceWeb.Tests.InfrastructureTests
{
    [TestFixture]
    public class GenericCategoryRepository_Test
    {
        #region Category Generic Repository Functions Test

        private AppDbContext _context;
        private GenericRepository<Category> _repository;

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
            _context.Set<Category>().AddRange(new List<Category>
            {
                new Category { Id = "1", Name = "Category 1" },
                new Category { Id = "2", Name = "Category 2" },
                new Category { Id = "3", Name = "Category 3" },
                new Category { Id = "4", Name = "Category 4" }
            });
            _context.SaveChanges();

            // initialize repository
            _repository = new GenericRepository<Category>(_context);
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
            Assert.That(result.Any(r => r.Name == "Category 1") , Is.True);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_InValid_Data()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(r => r.Name == "Cate 1"), Is.False);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_All_Data_Success()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            var expectedNames = new[] { "Category 1", "Category 2", "Category 3", "Category 4" };
            Assert.That(result.Select(r => r.Name), Is.EquivalentTo(expectedNames));
        }

        #endregion

        #region Get By Id Async Function Test
        [Test]
        public async Task GetByIdAsync_Test_Return_Valid_Data()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.Name, Is.EqualTo("Category 1"));
        }

        [Test]
        public async Task GetByIdAsync_Test_Return_InValid_Data()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.Name, Is.Not.EqualTo("Cate 2") , "Name does not match the expected value.");
        }

        [Test]
        public void GetByIdAsync_Test_Set_Empty_Id()
        {
            // Act & Assert
            Assert.ThatAsync(async()=>{ await _repository.GetByIdAsync(""); } , Is.Null , "Id Cann't be null");
        }

        #endregion

        #region Add Async Function Test

        [Test]
        public async Task AddAsync_Test_Add_Valid_Item()
        {
            // Arrange
            var item = new Category { Id = "5", Name = "Test Item" };

            // Act
            await _repository.AddAsync(item);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<Category>().FindAsync("5");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Test Item"));
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
            var item1 = new Category { Id = "6", Name = "Test Item 1" };
            var item2 = new Category { Id = "6", Name = "Duplicate Item" };

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

            existingItem!.Name = "Updated Name";

            // Act
            _repository.Update(existingItem);
            await _context.SaveChangesAsync();

            // Assert
            var updatedItem = await _context.Set<Category>().FindAsync("4");
            Assert.That(updatedItem, Is.Not.Null);
            Assert.That(updatedItem!.Name, Is.EqualTo("Updated Name"));
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
            var nonExistentItem = new Category { Id = "99", Name = "Non Existent Item" };
            
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
            var existingItem = new Category { Id = "5", Name = "Original Name" };
            await _context.Set<Category>().AddAsync(existingItem);
            await _context.SaveChangesAsync();

            // Act
            _repository.Update(existingItem);
            await _context.SaveChangesAsync();

            // Assert
            var unchangedItem = await _context.Set<Category>().FindAsync("5");
            Assert.That(unchangedItem, Is.Not.Null);
            Assert.That(unchangedItem!.Name, Is.EqualTo("Original Name"));
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
            var result = await _context.Set<Category>().FindAsync("4");
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

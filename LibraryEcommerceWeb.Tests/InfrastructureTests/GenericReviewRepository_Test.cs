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
    public class GenericReviewRepository_Test
    {
        #region Review Generic Repository Functions Test

        private AppDbContext _context;
        private GenericRepository<Review> _repository;

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
            _context.Set<Review>().AddRange(new List<Review>
            {
                new Review { Id = "1", Rating = 5, Comment = "Excellent product!", ProductId = "1", UserId = "1", CreatedAt = DateTime.UtcNow },
                new Review { Id = "2", Rating = 4, Comment = "Very good, but needs improvement.", ProductId = "2", UserId = "2", CreatedAt = DateTime.UtcNow },
                new Review { Id = "3", Rating = 3, Comment = "Good, but not as expected.", ProductId = "3", UserId = "1", CreatedAt = DateTime.UtcNow }
            });
            _context.SaveChanges();

            // initialize repository
            _repository = new GenericRepository<Review>(_context);
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
        public async Task GetAllAsync_Test_Return_Count_Of_All_Reviews()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllAsync_Test_Return_Valid_Reviews()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(r => r.Rating == 5), Is.True);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_Invalid_Reviews()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(r => r.Rating == 6), Is.False);
        }

        #endregion

        #region Get By Id Async Function Test
        [Test]
        public async Task GetByIdAsync_Test_Return_Valid_Review()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.Rating, Is.EqualTo(5));
        }

        [Test]
        public async Task GetByIdAsync_Test_Return_Invalid_Review()
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
        public async Task AddAsync_Test_Add_Valid_Review()
        {
            // Arrange
            var review = new Review { Id = "4", Rating = 5, Comment = "Amazing product, will buy again!", ProductId = "2", UserId = "3", CreatedAt = DateTime.UtcNow };

            // Act
            await _repository.AddAsync(review);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<Review>().FindAsync("4");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Comment, Is.EqualTo("Amazing product, will buy again!"));
        }

        [Test]
        public void AddAsync_Test_Add_Null_Review_Throws_Exception()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _repository.AddAsync(null!);
                await _context.SaveChangesAsync();
            });
        }

        [Test]
        public void AddAsync_Test_Add_Invalid_Rating_Throws_Exception()
        {
            // Arrange
            var review = new Review { Id = "5", Rating = 6, Comment = "Invalid rating", ProductId = "1", UserId = "2", CreatedAt = DateTime.UtcNow };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
            {
                // Explicitly trigger validation
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(review, new ValidationContext(review), validationResults, true);
                if (!isValid)
                {
                    throw new ValidationException(validationResults.FirstOrDefault()?.ErrorMessage);
                }

                // If validation passes, attempt to add and save
                _repository.AddAsync(review).Wait();
                _context.SaveChangesAsync().Wait();
            });

            Assert.That(exception?.Message, Is.EqualTo("The field Rating must be between 1 and 5."));
        }

        #endregion

        #region Update Async Function Test
        [Test]
        public async Task UpdateAsync_Test_Update_Valid_Review()
        {
            // Arrange
            var existingReview = await _repository.GetByIdAsync("2");
            existingReview!.Comment = "Updated comment";

            // Act
            _repository.Update(existingReview);
            await _context.SaveChangesAsync();

            // Assert
            var updatedReview = await _context.Set<Review>().FindAsync("2");
            Assert.That(updatedReview, Is.Not.Null);
            Assert.That(updatedReview!.Comment, Is.EqualTo("Updated comment"));
        }

        [Test]
        public void UpdateAsync_Test_Update_Null_Review_Throws_Exception()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _repository.Update(null!);
            });
        }

        [Test]
        public void UpdateAsync_Test_Update_NonExistent_Review()
        {
            // Arrange
            var nonExistentReview = new Review { Id = "99", Rating = 4, Comment = "This review does not exist", ProductId = "1", UserId = "2", CreatedAt = DateTime.UtcNow };

            // Act & Assert
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                _repository.Update(nonExistentReview);
                await _context.SaveChangesAsync();
            });
        }

        #endregion

        #region Delete Async Function Test
        [Test]
        public async Task DeleteAsync_Test_Delete_Existing_Review_Success()
        {
            // Arrange
            var reviewToDelete = await _repository.GetByIdAsync("3");

            // Act
            await _repository.DeleteAsync("3");
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<Review>().FindAsync("3");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteAsync_Test_Delete_NonExistent_Review_No_Exception()
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

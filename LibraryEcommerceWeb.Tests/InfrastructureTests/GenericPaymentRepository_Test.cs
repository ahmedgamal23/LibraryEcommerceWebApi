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
    public class GenericPaymentRepository_Test
    {
        #region Payment Generic Repository Functions Test

        private AppDbContext _context;
        private GenericRepository<Payment> _repository;

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
            _context.Set<Payment>().AddRange(new List<Payment>
            {
                new Payment { Id = "1", OrderId = "order1", Amount = 150.00m, PaymentStatus = "Paid", PaymentMethod = "Stripe", PaymentDate = DateTime.UtcNow },
                new Payment { Id = "2", OrderId = "order2", Amount = 100.00m, PaymentStatus = "Failed", PaymentMethod = "PayPal", PaymentDate = DateTime.UtcNow },
                new Payment { Id = "3", OrderId = "order3", Amount = 250.00m, PaymentStatus = "Pending", PaymentMethod = "Stripe", PaymentDate = DateTime.UtcNow },
                new Payment { Id = "4", OrderId = "order4", Amount = 500.00m, PaymentStatus = "Paid", PaymentMethod = "PayPal", PaymentDate = DateTime.UtcNow }
            });
            _context.SaveChanges();

            // initialize repository
            _repository = new GenericRepository<Payment>(_context);
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
            Assert.That(result.Any(r => r.PaymentMethod == "Stripe"), Is.True);
        }

        [Test]
        public async Task GetAllAsync_Test_Return_Invalid_Data()
        {
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.That(result.Any(r => r.PaymentStatus == "Completed"), Is.False);
        }

        #endregion

        #region Get By Id Async Function Test
        [Test]
        public async Task GetByIdAsync_Test_Return_Valid_Data()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");
            // Assert
            Assert.That(result!.Amount, Is.EqualTo(150.00m));
        }

        [Test]
        public async Task GetByIdAsync_Test_Return_Invalid_Data()
        {
            // Act
            var result = await _repository.GetByIdAsync("2");
            // Assert
            Assert.That(result!.Amount, Is.Not.EqualTo(500.00m));
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
        public async Task AddAsync_Test_Add_Valid_Payment()
        {
            // Arrange
            var payment = new Payment { Id = "5", OrderId = "order5", Amount = 350.00m, PaymentStatus = "Paid", PaymentMethod = "Stripe", PaymentDate = DateTime.UtcNow };

            // Act
            await _repository.AddAsync(payment);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<Payment>().FindAsync("5");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Amount, Is.EqualTo(350.00m));
        }

        [Test]
        public void AddAsync_Test_Add_Null_Payment_Throws_Exception()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _repository.AddAsync(null!);
                await _context.SaveChangesAsync();
            });
        }

        [Test]
        public async Task AddAsync_Test_Add_Duplicate_Payment()
        {
            // Arrange
            var payment1 = new Payment { Id = "6", OrderId = "order6", Amount = 200.00m, PaymentStatus = "Paid", PaymentMethod = "PayPal", PaymentDate = DateTime.UtcNow };
            var payment2 = new Payment { Id = "6", OrderId = "order7", Amount = 300.00m, PaymentStatus = "Pending", PaymentMethod = "Stripe", PaymentDate = DateTime.UtcNow };

            await _repository.AddAsync(payment1);
            await _context.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _repository.AddAsync(payment2);
                await _context.SaveChangesAsync();
            });
        }

        #endregion

        #region Update Async Function Test
        [Test]
        public async Task UpdateAsync_Test_Update_Valid_Payment()
        {
            // Arrange
            var existingPayment = await _repository.GetByIdAsync("4");
            existingPayment!.Amount = 600.00m;

            // Act
            _repository.Update(existingPayment);
            await _context.SaveChangesAsync();

            // Assert
            var updatedPayment = await _context.Set<Payment>().FindAsync("4");
            Assert.That(updatedPayment, Is.Not.Null);
            Assert.That(updatedPayment!.Amount, Is.EqualTo(600.00m));
        }

        [Test]
        public void UpdateAsync_Test_Update_Null_Payment_Throws_Exception()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _repository.Update(null!);
            });
        }

        [Test]
        public void UpdateAsync_Test_Update_NonExistent_Payment()
        {
            // Arrange
            var nonExistentPayment = new Payment { Id = "99", OrderId = "order99", Amount = 400.00m, PaymentStatus = "Paid", PaymentMethod = "PayPal", PaymentDate = DateTime.UtcNow };

            // Act & Assert
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                _repository.Update(nonExistentPayment);
                await _context.SaveChangesAsync();
            });
        }

        #endregion

        #region Delete Async Function Test
        [Test]
        public async Task DeleteAsync_Test_Delete_Existing_Payment_Success()
        {
            // Arrange
            var paymentToDelete = await _repository.GetByIdAsync("4");

            // Act
            await _repository.DeleteAsync("4");
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Set<Payment>().FindAsync("4");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteAsync_Test_Delete_NonExistent_Payment_No_Exception()
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

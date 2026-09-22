using EcommerceApi.Exceptions;
using EcommerceApi.Features.Base;
using EcommerceApi.Features.Orders.Commands.Checkout;
using EcommerceApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace EcommerceApi.Tests.Features
{
    public class CheckoutCommandHandlerTests
    {
        private readonly ICurrentUserService _currentUserService;
        public CheckoutCommandHandlerTests()
        {
            _currentUserService = NSubstitute.Substitute.For<ICurrentUserService>();
        }
        private AppDbContext GetInMemoryDbcontext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task Handle_Should_Throw_ApiException_When_User_Is_Unauthorized()
        {
            _currentUserService.UserId.Returns((int?)null);
            using var context = GetInMemoryDbcontext();
            var handler = new CheckoutCommandHandler(context, _currentUserService);
            var command = new CheckoutCommand { UserId = 1 };
            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ApiException>()
                .Where(e => e.StatusCode == StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task Handle_Should_Throw_ApiException_When_Cart_Is_Empty()
        {
            _currentUserService.UserId.Returns(1);
            using var context = GetInMemoryDbcontext();
            var handler = new CheckoutCommandHandler(context, _currentUserService);
            var command = new CheckoutCommand { UserId = 1 };
            var act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>()
                .Where(e => e.StatusCode == StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Handle_Should_Throw_ApiException_When_Stock_Is_Insufficient()
        {
            _currentUserService.UserId.Returns(1);
            using var context = GetInMemoryDbcontext();
            var user = new User
            {
                Id = 1,
                Username = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hash",
                Role = "Customer"
            };
            context.Users.Add(user);
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 50.00m,
                StockQuantity = 3
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();
            var cart = new Cart
            {
                UserId = 1,
                CartItems = new List<CartItem>
                    {
                        new CartItem { Product = product, Quantity = 5 }
                    }
            };
            context.Carts.Add(cart);
            await context.SaveChangesAsync();

            var handler = new CheckoutCommandHandler(context, _currentUserService);
            var command = new CheckoutCommand { UserId = 1 };

            var act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>();
        }

        [Fact]
        public async Task Handle_Should_Checkout_Successfully_When_Valid()
        {
            _currentUserService.UserId.Returns(1);
            using var context = GetInMemoryDbcontext();
            var user = new User
            {
                Id = 1,
                Username = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hash",
                Role = "Customer"
            };
            context.Users.Add(user);
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 1000.00m,
                StockQuantity = 10
            };
            context.Products.Add(product);
            var cart = new Cart
            {
                UserId = 1,
                CartItems = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Quantity = 2 }
                }
            };
            context.Carts.Add(cart);
            await context.SaveChangesAsync();
            var handler = new CheckoutCommandHandler(context, _currentUserService);
            var command = new CheckoutCommand { UserId = 1 };

            var result = await handler.Handle(command, CancellationToken.None);
            result.Should().NotBeNull();
            result.TotalAmount.Should().Be(2000.00m);
            result.Items.Should().HaveCount(1);
            var updatedProduct = await context.Products.FindAsync(1);
            await context.Entry(updatedProduct).ReloadAsync();
            updatedProduct.StockQuantity.Should().Be(8);
            var remainingCartItems = await context.CartItems.ToListAsync();
            remainingCartItems.Should().BeEmpty();
        }
    }
}
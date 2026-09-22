using EcommerceApi.Exceptions;
using EcommerceApi.Features.Products.Commands.CreateProduct;
using EcommerceApi.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Tests.Features
{
    public class CreateProductCommandHandlerTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
        [Fact] 
        public async Task Handle_Should_Create_Product_Successfully_When_Valid()
        {
            using var context = GetInMemoryDbContext();
            var handler = new CreateProductCommandHandler(context);
            var command = new CreateProductCommand 
            { 
                Name = "Gaming Keyboard",
                Description = "Test Description", 
                Price = 89.99m, StockQuantity = 15,
                Sku = "KB-RGB-001" 
            };
            var response = await handler.Handle(command, CancellationToken.None);
            response.Should().NotBeNull();
            response.Name.Should().Be("Gaming Keyboard"); 
            response.Price.Should().Be(89.99m); 
            var productInDb = await context.Products.FirstOrDefaultAsync(p => p.Id == response.Id);
            productInDb.Should().NotBeNull();
            productInDb!.Description.Should().Be("Test Description"); 
            productInDb.Sku.Should().Be("KB-RGB-001");
        }
        [Fact]
        public async Task Handle_Should_Throw_ApiException_When_Sku_Is_Duplicate()
        {
            using var context = GetInMemoryDbContext();
            context.Products.Add(new Product {
                Id = 1,
                Name = "Existing Item",
                Sku = "DUPLICATE-SKU",
                Price = 10,
                StockQuantity = 5 
            });
            await context.SaveChangesAsync();
            var handler = new CreateProductCommandHandler(context);
            var command = new CreateProductCommand
            {
                Name = "New Item",
                Price = 20,
                StockQuantity = 10,
                Sku = "DUPLICATE-SKU" // Duplicate SKU [2]
            };
            var act = async() => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage($"A product with SKU '{command.Sku}' already exists.");
        }

    }
}

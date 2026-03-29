using FluentAssertions;
using InventarioApi.DTOs;
using InventarioApi.Models;
using InventarioApi.Repositories;
using InventarioApi.Services;
using Moq;

namespace InventarioApi.Tests.Services;

public class SaleServiceTests
{
    private static Product MakeProduct(int id, int stock) => new()
    {
        Id = id,
        Name = $"Producto {id}",
        Price = 50.00m,
        CurrentStock = stock,
        MinimumStock = 5,
        CreatedAt = DateTime.Now
    };
    [Fact]
    public async Task CreateAsync_StockSuficiente_DescuentaStockYCreaSale()
    {
        var product = MakeProduct(id: 1, stock: 20);
        var request = new CreateSaleRequest(new List<SaleItemRequest>
        {
            new(ProductId: 1, Quantity: 3)
        });

        var createdSale = new Sale
        {
            Id = 1,
            UserId = 1,
            TotalAmount = 150.00m,
            SaleDate = DateTime.UtcNow,
            User = new User { Id = 1, Username = "admin" },
            SaleItems = new List<SaleItem>
            {
                new() { ProductId = 1, Quantity = 3, UnitPrice = 50.00m,
                         Product = product }
            }
        };

        var productRepoMock = new Mock<IProductRepository>();
        productRepoMock.Setup(r => r.GetByIdAsync(1))
                       .ReturnsAsync(product);
        productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                       .ReturnsAsync((Product p) => p);
        var saleRepoMock = new Mock<ISaleRepository>();
        saleRepoMock.Setup(r => r.CreateAsync(It.IsAny<Sale>()))
                    .ReturnsAsync(createdSale);
        saleRepoMock.Setup(r => r.GetByIdAsync(1))
                    .ReturnsAsync(createdSale);

        var service = new SaleService(saleRepoMock.Object, productRepoMock.Object);
        var result = await service.CreateAsync(request, userId: 1);

        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(150.00m);
        result.Items.Should().HaveCount(1);
        result.Items[0].Quantity.Should().Be(3);
        result.Items[0].UnitPrice.Should().Be(50.00m);
        result.Items[0].Subtotal.Should().Be(150.00m);

        productRepoMock.Verify(r => r.UpdateAsync(
            It.Is<Product>(p => p.CurrentStock == 17)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_StockInsuficiente_LanzaInvalidOperationException()
    {
        // producto con solo 2 unidades y pedir 10
        var product = MakeProduct(id: 1, stock: 2);
        var request = new CreateSaleRequest(new List<SaleItemRequest>
        {
            new(ProductId: 1, Quantity: 10)
        });

        var productRepoMock = new Mock<IProductRepository>();
        productRepoMock.Setup(r => r.GetByIdAsync(1))
                       .ReturnsAsync(product);

        var saleRepoMock = new Mock<ISaleRepository>();
        var service = new SaleService(saleRepoMock.Object, productRepoMock.Object);
        var act = async () => await service.CreateAsync(request, userId: 1);
        await act.Should()
                 .ThrowAsync<InvalidOperationException>()
                 .WithMessage("*insuficiente*");
        saleRepoMock.Verify(r => r.CreateAsync(It.IsAny<Sale>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ProductoNoExiste_LanzaKeyNotFoundException()
    {
        var request = new CreateSaleRequest(new List<SaleItemRequest>
        {
            new(ProductId: 99, Quantity: 1)
        });

        var productRepoMock = new Mock<IProductRepository>();
        productRepoMock.Setup(r => r.GetByIdAsync(99))
                       .ReturnsAsync((Product?)null);

        var saleRepoMock = new Mock<ISaleRepository>();
        var service = new SaleService(saleRepoMock.Object, productRepoMock.Object);
        var act = async () => await service.CreateAsync(request, userId: 1);

        await act.Should()
                 .ThrowAsync<KeyNotFoundException>()
                 .WithMessage("*99*");
    }
}
using FluentAssertions;
using InventarioApi.DTOs;
using InventarioApi.Models;
using InventarioApi.Repositories;
using InventarioApi.Services;
using Moq;

namespace InventarioApi.Tests.Services;

public class ProductServiceTests
{
    private static Product MakeProduct(int id = 1, int currentStock = 10, int minimumStock = 5) =>
        new()
        {
            Id = id,
            Name = $"Producto {id}",
            Price = 100.00m,
            CurrentStock = currentStock,
            MinimumStock = minimumStock,
            CreatedAt = DateTime.Now
        };

    [Fact]
    public async Task GetByIdAsync_ProductoExiste_DevuelveProductoCorrectamente()
    {
        var product = MakeProduct(id: 1, currentStock: 10, minimumStock: 5);

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);
        var service = new ProductService(repoMock.Object);
        var result = await service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Producto 1");
        result.Price.Should().Be(100.00m);
        result.RequiresRestocking.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_ProductoNoExiste_LanzaKeyNotFoundException()
    {
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Product?)null); 
        var service = new ProductService(repoMock.Object);
        var act = async () => await service.GetByIdAsync(99);
        await act.Should()
                 .ThrowAsync<KeyNotFoundException>()
                 .WithMessage("*99*");
    }

    [Fact]
    public async Task CreateAsync_DatosValidos_CreaProductoYDevuelveRespuesta()
    {
        var request = new CreateProductRequest(
            Name: "Teclado Nuevo",
            Price: 75.00m,
            CurrentStock: 20,
            MinimumStock: 10);

        var savedProduct = new Product
        {
            Id = 10,
            Name = request.Name,
            Price = request.Price,
            CurrentStock = request.CurrentStock,
            MinimumStock = request.MinimumStock,
            CreatedAt = DateTime.Now
        };

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Product>()))
                .ReturnsAsync(savedProduct);

        var service = new ProductService(repoMock.Object);
        var result = await service.CreateAsync(request);

        result.Should().NotBeNull();
        result.Id.Should().Be(10);
        result.Name.Should().Be("Teclado Nuevo");
        result.Price.Should().Be(75.00m);
        result.RequiresRestocking.Should().BeFalse();

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task GetLowStockAsync_HayProductosBajos_DevuelveSoloLosQueRequierenReposicion()
    {

        var products = new List<Product>
        {
            MakeProduct(id: 1, currentStock: 2,  minimumStock: 10),
            MakeProduct(id: 2, currentStock: 50, minimumStock: 20),
            MakeProduct(id: 3, currentStock: 1,  minimumStock: 8),
        }
        .Where(p => p.CurrentStock < p.MinimumStock)
        .ToList();

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetLowStockAsync())
                .ReturnsAsync(products);

        var service = new ProductService(repoMock.Object);
        var result = (await service.GetLowStockAsync()).ToList();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.StockDeficit > 0);
        result.First(p => p.Id == 1).StockDeficit.Should().Be(8);
        result.First(p => p.Id == 3).StockDeficit.Should().Be(7);
    }

    [Fact]
    public async Task DeleteAsync_ProductoNoExiste_LanzaKeyNotFoundException()
    {
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.DeleteAsync(99))
                .ReturnsAsync(false); 

        var service = new ProductService(repoMock.Object);
        var act = async () => await service.DeleteAsync(99);
        await act.Should()
                 .ThrowAsync<KeyNotFoundException>()
                 .WithMessage("*99*");
    }
}
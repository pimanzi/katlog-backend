using katlog_backend.DTOs;
using katlog_backend.Enums;
using katlog_backend.Exceptions;
using katlog_backend.Models;
using katlog_backend.Repositories.Interfaces;
using katlog_backend.Services;
using Moq;

namespace katlog_backend.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<IBrandRepository> _brandRepo = new();
    private readonly Mock<ICategoryRepository> _categoryRepo = new();

    private ProductService CreateService() =>
        new(_productRepo.Object, _brandRepo.Object, _categoryRepo.Object);

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenProductDoesNotExist()
    {
        _productRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(99));
    }

    [Fact]
    public async Task CreateAsync_ThrowsConflictException_WhenProductCodeAlreadyExists()
    {
        _productRepo.Setup(r => r.ProductCodeExistsAsync("DUPE-01")).ReturnsAsync(true);

        var service = CreateService();

        var dto = new CreateProductDto
        {
            Name = "Test Product",
            ProductCode = "DUPE-01",
            Description = "A test product",
            Season = Season.Summer,
            TargetMarket = ["Men"],
            BrandId = 1,
            CategoryId = 1
        };

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(dto));
    }
}

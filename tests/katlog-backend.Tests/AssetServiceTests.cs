using katlog_backend.Enums;
using katlog_backend.Exceptions;
using katlog_backend.Models;
using katlog_backend.Repositories.Interfaces;
using katlog_backend.Services;
using Moq;

namespace katlog_backend.Tests;

public class AssetServiceTests
{
    [Fact]
    public async Task ApproveAsync_ThrowsConflictException_WhenAssetIsNotInPendingReview()
    {
        var mockRepo = new Mock<IAssetRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Asset
        {
            Id = 1,
            OriginalFileName = "photo.jpg",
            FileName = "photo",
            ContentType = "image/jpeg",
            FileUrl = "https://res.cloudinary.com/demo/photo.jpg",
            UploadedBy = "user-123",
            Status = AssetStatus.Uploaded,  
            ProductId = 1
        });

        
        var service = new AssetService(
            mockRepo.Object,
            new Mock<IProductRepository>().Object,
            new Mock<IVariantRepository>().Object,
            null!,
            null!);

        await Assert.ThrowsAsync<ConflictException>(() => service.ApproveAsync(1, "user-123"));
    }
}

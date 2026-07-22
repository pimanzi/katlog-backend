using Katlog.Api.Repositories;
using Katlog.Api.Repositories.Interfaces;
using Katlog.Api.Services;
using Katlog.Api.Services.Interfaces;

namespace Katlog.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IVariantRepository, VariantRepository>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        return services;
    }

    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
       
        services.AddScoped<IAssetService, AssetService>();
        services.AddScoped<IVariantService, VariantService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReadinessService, ReadinessService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
}
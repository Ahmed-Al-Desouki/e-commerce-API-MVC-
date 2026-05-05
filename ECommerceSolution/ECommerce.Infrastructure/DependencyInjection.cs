using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Infrastructure.Configurations;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.Configure<PaymobOptions>(configuration.GetSection(PaymobOptions.SectionName));
            services.Configure<PaymentFlowOptions>(configuration.GetSection(PaymentFlowOptions.SectionName));
            services.AddHttpClient<IPaymentGateway, PaymobGateway>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<PaymobOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
            services.AddSingleton<IMerchantOrderIdGenerator, MerchantOrderIdGenerator>();
            services.AddSingleton<IPaymentFlowSettings, PaymentFlowSettings>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}

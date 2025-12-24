using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfwork;
using Application.Mapping;
using Application.Services;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using StackExchange.Redis;

namespace Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<FakebookContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddMemoryCache();

            string redisConn;
            var redisSection = configuration.GetSection("Redis:ConnectionString");
            if (redisSection.Exists())
            {
                redisConn = redisSection.Value;
            }
            else
            {
                redisConn = "localhost:6379";
            }
            var options = ConfigurationOptions.Parse(redisConn);
            options.AbortOnConnectFail = false; 
            options.ConnectRetry = 5;
            options.ConnectTimeout = 5000;
            options.SyncTimeout = 5000;
            options.AllowAdmin = false;

            var mux = ConnectionMultiplexer.Connect(options);
            services.AddSingleton<IConnectionMultiplexer>(mux);
            services.AddSingleton<IRedisService, RedisService>();

            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}

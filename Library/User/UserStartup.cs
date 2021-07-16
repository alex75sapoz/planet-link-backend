global using Library.Base;
global using Library.User.Contract;
global using Library.User.Entity;
global using Library.User.Enum;
global using Library.User.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.User
{
    public interface IUserStartup
    {
        public static void ConfigureServices(IServiceCollection services, UserConfiguration configuration, string databaseConnection) =>
            UserStartup.ConfigureServices(services, configuration, databaseConnection);

        public static async Task LoadMemoryCacheAsync(IServiceProvider serviceProvider) =>
            await UserMemoryCache.LoadAsync(serviceProvider.GetRequiredService<UserRepository>());

        public static object GetStatus() =>
            UserStartup.GetStatus();
    }

    static class UserStartup
    {
        public static bool IsReady { get; private set; }

        public static void ConfigureServices(IServiceCollection services, UserConfiguration configuration, string databaseConnection)
        {
            if (IsReady) return;

            services
                //Internal
                .AddDbContext<UserContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<UserRepository>()
                .AddTransient<UserService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<IUserService, UserService>();

            IsReady = true;
        }

        public static object GetStatus() => new
        {
            IsReady,
            IsMemoryCacheReady = UserMemoryCache.IsReady,
            RegisteredType = new
            {
                Internal = new[]
                {
                    nameof(UserContext),
                    nameof(UserRepository),
                    nameof(UserService),
                    nameof(UserMemoryCache),
                    nameof(UserConfiguration)
                },
                Public = new[]
                {
                    nameof(IUserRepository),
                    nameof(IUserService),
                    nameof(IUserMemoryCache)
                }
            },
            MemoryCache = new
            {
                TotalUserTypes = UserMemoryCache.UserTypes.Count,
                TotalSessions = UserMemoryCache.UserSessions.Count,
                TotalUsers = UserMemoryCache.Users.Count,
            }
        };
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.User
{
    public interface IUserStartup
    {
        public static bool IsMemoryCacheReady =>
            UserStartup.IsMemoryCacheReady;

        public static void Startup(IServiceCollection services, UserConfiguration configuration, string databaseConnection) =>
            UserStartup.Startup(services, configuration, databaseConnection);

        public static async Task RefreshMemoryCacheAsync(IServiceProvider serviceProvider) =>
            await UserStartup.RefreshMemoryCacheAsync(serviceProvider.GetRequiredService<UserRepository>());

        public static object GetStatus() =>
            UserStartup.GetStatus();
    }

    internal static class UserStartup
    {
        public static bool IsStarted { get; set; }
        public static bool IsMemoryCacheReady { get; set; }

        public static void Startup(IServiceCollection services, UserConfiguration configuration, string databaseConnection)
        {
            IsStarted = false;

            services
                //Internal
                .AddDbContext<UserContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<UserRepository>()
                .AddTransient<UserService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<IUserService, UserService>();

            IsStarted = true;
        }

        public static async Task RefreshMemoryCacheAsync(UserRepository repository)
        {
            IsMemoryCacheReady = false;

            var userTypeEntities = await repository.GetUserTypesAsync();
            var userEntities = await repository.GetUsersAsync();
            var userSessionEntities = await repository.GetUserSessionsAsync();

            UserMemoryCache.UserTypes.Clear();
            foreach (var userType in userTypeEntities.Select(userTypeEntity => userTypeEntity.MapToTypeContract()))
                UserMemoryCache.UserTypes.TryAdd(userType.TypeId, userType);

            UserMemoryCache.Users.Clear();
            foreach (var user in userEntities.Select(userEntity => userEntity.MapToUserContract()))
                UserMemoryCache.Users.TryAdd(user.UserId, user);

            UserMemoryCache.UserSessions.Clear();
            foreach (var userSession in userSessionEntities.Select(userSessionEntity => userSessionEntity.MapToSessionContract()))
                UserMemoryCache.UserSessions.TryAdd(userSession.UserSessionId, userSession);

            IsMemoryCacheReady = true;
        }

        public static object GetStatus() => new
        {
            IsStarted,
            IsMemoryCacheReady,
            RegisteredType = new
            {
                Internal = new[]
                {
                    nameof(UserContext),
                    nameof(UserRepository),
                    nameof(UserService),
                    nameof(UserConfiguration)
                },
                Public = new[]
                {
                    nameof(IUserRepository),
                    nameof(IUserService)
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
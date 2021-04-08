using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Library.User
{
    public interface IUserStartup
    {
        public static bool IsMemoryCacheReady =>
            UserStartup.IsMemoryCacheReady;

        public static void Startup(IServiceCollection services, Func<Type, object> configuration, string databaseConnection) =>
            UserStartup.Startup(services, (UserConfiguration)configuration(typeof(UserConfiguration)), databaseConnection);

        public static async Task RefreshMemoryCacheAsync(IServiceProvider serviceProvider) =>
            await UserStartup.RefreshMemoryCacheAsync(serviceProvider.GetRequiredService<UserRepository>());
    }

    internal static class UserStartup
    {
        public static bool IsMemoryCacheReady { get; set; }

        public static void Startup(IServiceCollection services, UserConfiguration configuration, string databaseConnection) =>
            services
                .AddDbContext<UserContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<UserRepository>()
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<IUserService, UserService>()
                .AddSingleton(configuration);

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
    }
}
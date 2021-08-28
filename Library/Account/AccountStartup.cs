global using Library.Account.Contract;
global using Library.Account.Entity;
global using Library.Account.Enum;
global using Library.Account.Response;
global using Library.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Account
{
    public interface IAccountStartup
    {
        public static void ConfigureServices(IServiceCollection services, AccountConfiguration configuration, string databaseConnection) =>
            AccountStartup.ConfigureServices(services, configuration, databaseConnection);

        public static object GetStatus() =>
            AccountStartup.GetStatus();
    }

    static class AccountStartup
    {
        public static bool IsReady { get; private set; }

        public static void ConfigureServices(IServiceCollection services, AccountConfiguration configuration, string databaseConnection)
        {
            if (IsReady) return;

            services
                //Internal
                .AddDbContext<AccountContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<AccountRepository>()
                .AddTransient<AccountService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IAccountRepository, AccountRepository>()
                .AddTransient<IAccountService, AccountService>();

            IsReady = true;
        }

        public static object GetStatus() => new
        {
            IsReady,
            RegisteredType = new
            {
                Internal = new[]
                {
                    nameof(AccountContext),
                    nameof(AccountRepository),
                    nameof(AccountService),
                    nameof(AccountConfiguration)
                },
                Public = new[]
                {
                    nameof(IAccountRepository),
                    nameof(IAccountMemoryCache),
                    nameof(IAccountService)
                }
            },
            MemoryCache = new
            {
                TotalUserTypes = IAccountMemoryCache.UserTypes.Count,
                TotalUserGenders = IAccountMemoryCache.UserGenders.Count,
                TotalUserSessions = IAccountMemoryCache.UserSessions.Count,
                TotalUsers = IAccountMemoryCache.Users.Count,
            }
        };
    }
}
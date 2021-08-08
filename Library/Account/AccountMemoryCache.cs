using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Account
{
    static class AccountMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, AccountUserTypeContract> UserTypes = new();
        public static readonly ConcurrentDictionary<int, AccountUserGenderContract> UserGenders = new();
        public static readonly ConcurrentDictionary<int, AccountUserContract> Users = new();
        public static readonly ConcurrentDictionary<int, AccountUserSessionContract> UserSessions = new();

        public static async Task LoadAsync(AccountRepository repository)
        {
            if (IsReady) return;

            var userTypes = (await repository.GetUserTypesAsync()).Select(userTypeEntity => userTypeEntity.MapToUserTypeContract()).ToList();
            var userGenders = (await repository.GetUserGendersAsync()).Select(userGenderEntity => userGenderEntity.MapToUserGenderContract()).ToList();
            var users = (await repository.GetUsersAsync()).Select(userEntity => userEntity.MapToUserContract()).ToList();
            var userSessions = (await repository.GetUserSessionsAsync()).Select(userSessionEntity => userSessionEntity.MapToUserSessionContract()).ToList();

            foreach (var userType in userTypes)
                UserTypes[userType.UserTypeId] = userType;

            foreach (var userGender in userGenders)
                UserGenders[userGender.UserGenderId] = userGender;

            foreach (var user in users)
                Users[user.UserId] = user;

            foreach (var userSession in userSessions)
                UserSessions[userSession.UserSessionId] = userSession;

            IsReady = true;
        }
    }
}
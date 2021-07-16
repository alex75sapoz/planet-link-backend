using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.User
{
    public interface IUserMemoryCache
    {
        public static IReadOnlyDictionary<int, UserContract> Users => UserMemoryCache.Users;
        public static IReadOnlyDictionary<int, UserSessionContract> UserSessions => UserMemoryCache.UserSessions;
        public static IReadOnlyDictionary<int, UserTypeContract> UserTypes => UserMemoryCache.UserTypes;
    }

    static class UserMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, UserTypeContract> UserTypes = new();
        public static readonly ConcurrentDictionary<int, UserContract> Users = new();
        public static readonly ConcurrentDictionary<int, UserSessionContract> UserSessions = new();

        public static async Task LoadAsync(UserRepository repository)
        {
            if (IsReady) return;

            var userTypes = (await repository.GetUserTypesAsync()).Select(userTypeEntity => userTypeEntity.MapToTypeContract()).ToList();
            var users = (await repository.GetUsersAsync()).Select(userEntity => userEntity.MapToUserContract()).ToList();
            var userSessions = (await repository.GetUserSessionsAsync()).Select(userSessionEntity => userSessionEntity.MapToSessionContract()).ToList();

            foreach (var userType in userTypes)
                UserTypes[userType.UserTypeId] = userType;

            foreach (var user in users)
                Users[user.UserId] = user;

            foreach (var userSession in userSessions)
                UserSessions[userSession.UserSessionId] = userSession;

            IsReady = true;
        }
    }
}
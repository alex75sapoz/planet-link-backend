using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.User
{
    public interface IUserMemoryCache
    {
        public static bool IsReady => UserMemoryCache.IsReady;

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

        public static async Task RefreshAsync(UserRepository repository)
        {
            var userTypes = (await repository.GetUserTypesAsync()).Select(userTypeEntity => userTypeEntity.MapToTypeContract()).ToDictionary(userType => userType.UserTypeId);
            var users = (await repository.GetUsersAsync()).Select(userEntity => userEntity.MapToUserContract()).ToDictionary(user => user.UserId);
            var userSessions = (await repository.GetUserSessionsAsync()).Select(userSessionEntity => userSessionEntity.MapToSessionContract()).ToDictionary(userSession => userSession.UserSessionId);

            //UserTypes
            foreach (var userType in userTypes)
                UserTypes[userType.Key] = userType.Value;

            foreach (var userType in UserTypes.Where(userType => !userTypes.ContainsKey(userType.Key)).ToList())
                UserTypes.TryRemove(userType);

            //Users
            foreach (var user in users)
                Users[user.Key] = user.Value;

            foreach (var user in Users.Where(user => !users.ContainsKey(user.Key)).ToList())
                Users.TryRemove(user);

            //UserSessions
            foreach (var userSession in userSessions)
                UserSessions[userSession.Key] = userSession.Value;

            foreach (var userSession in UserSessions.Where(userSession => !userSessions.ContainsKey(userSession.Key)).ToList())
                UserSessions.TryRemove(userSession);

            IsReady = true;
        }
    }
}
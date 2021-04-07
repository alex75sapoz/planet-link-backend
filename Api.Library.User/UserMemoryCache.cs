using Api.Library.User.Contract;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Api.Library.User
{
    public interface IUserMemoryCache
    {
        public static IReadOnlyDictionary<int, UserContract> Users => UserMemoryCache.Users;
        public static IReadOnlyDictionary<int, UserSessionContract> UserSessions => UserMemoryCache.UserSessions;
        public static IReadOnlyDictionary<int, UserTypeContract> UserTypes => UserMemoryCache.UserTypes;
    }

    internal static class UserMemoryCache
    {
        public static readonly ConcurrentDictionary<int, UserTypeContract> UserTypes = new();
        public static readonly ConcurrentDictionary<int, UserContract> Users = new();
        public static readonly ConcurrentDictionary<int, UserSessionContract> UserSessions = new();
    }
}
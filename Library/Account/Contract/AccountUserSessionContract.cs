using System;
using System.Text.Json.Serialization;

namespace Library.Account.Contract
{
    public class AccountUserSessionContract
    {
        [JsonIgnore]
        public bool IsAuthenticated => !IsExpired;
        [JsonIgnore]
        public bool IsExpired => TokenExpiresOn <= DateTimeOffset.Now;
        [JsonIgnore]
        public bool IsAboutToExpire => (TokenExpiresOn - DateTimeOffset.Now).TotalMinutes <= 5;
        [JsonIgnore]
        public int UserId { get; internal set; }

        public int UserSessionId { get; internal set; }
        public string Token { get; internal set; } = default!;
        public DateTimeOffset TokenExpiresOn { get; internal set; }

        public AccountUserContract User => IAccountMemoryCache.Users[UserId];
    }
}
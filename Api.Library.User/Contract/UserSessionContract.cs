using System;
using System.Text.Json.Serialization;

namespace Api.Library.User.Contract
{
    public class UserSessionContract
    {
        [JsonIgnore]
        public string RefreshToken { get; internal set; }
        [JsonIgnore]
        public bool IsAuthenticated => !IsExpired;
        [JsonIgnore]
        public bool IsExpired => TokenExpiresOn <= DateTimeOffset.Now;
        [JsonIgnore]
        public bool IsAboutToExpire => (TokenExpiresOn - DateTimeOffset.Now).TotalMinutes <= 5;

        public int UserSessionId { get; internal set; }
        public string Token { get; internal set; }
        public DateTimeOffset TokenExpiresOn { get; internal set; }

        public UserContract User { get; internal set; }
    }
}
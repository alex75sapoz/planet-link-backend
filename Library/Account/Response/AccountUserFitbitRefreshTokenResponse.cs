using RestSharp.Deserializers;

namespace Library.Account.Response
{
    class AccountUserFitbitRefreshTokenResponse
    {
        [DeserializeAs(Name = "access_token")]
        public string Token { get; set; } = default!;

        [DeserializeAs(Name = "expires_in")]
        public int TokenDurationInSeconds { get; set; }

        [DeserializeAs(Name = "refresh_token")]
        public string? RefreshToken { get; set; }
    }
}
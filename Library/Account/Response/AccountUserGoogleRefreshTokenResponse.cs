using RestSharp.Deserializers;

namespace Library.Account.Response
{
    class AccountUserGoogleRefreshTokenResponse
    {
        [DeserializeAs(Name = "id_token")]
        public string UserJsonWebToken { get; set; } = default!;

        [DeserializeAs(Name = "access_token")]
        public string Token { get; set; } = default!;

        [DeserializeAs(Name = "token_type")]
        public string TokenType { get; set; } = default!;

        [DeserializeAs(Name = "refresh_token")]
        public string RefreshToken { get; set; } = default!;

        [DeserializeAs(Name = "expires_in")]
        public int TokenDurationInSeconds { get; set; }

        [DeserializeAs(Name = "scope")]
        public string Permission { get; set; } = default!;
    }
}
using RestSharp.Deserializers;

namespace Library.User.Response
{
    internal class UserGoogleRefreshTokenResponse
    {
        [DeserializeAs(Name = "id_token")]
        public string UserJsonWebToken { get; set; }

        [DeserializeAs(Name = "access_token")]
        public string Token { get; set; }

        [DeserializeAs(Name = "token_type")]
        public string TokenType { get; set; }

        [DeserializeAs(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        [DeserializeAs(Name = "expires_in")]
        public int TokenDurationInSeconds { get; set; }

        [DeserializeAs(Name = "scope")]
        public string Permission { get; set; }
    }
}
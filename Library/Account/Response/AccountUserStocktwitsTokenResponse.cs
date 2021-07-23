using RestSharp.Deserializers;

namespace Library.Account.Response
{
    class AccountUserStocktwitsTokenResponse
    {
        [DeserializeAs(Name = "user_id")]
        public int UserId { get; set; }

        [DeserializeAs(Name = "access_token")]
        public string Token { get; set; } = default!;
    }
}
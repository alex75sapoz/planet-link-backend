using RestSharp.Deserializers;

namespace Library.User.Response
{
    internal class UserStocktwitsTokenResponse
    {
        public UserStocktwitsTokenResponse()
        {
            Token = default!;
        }

        [DeserializeAs(Name = "user_id")]
        public int UserId { get; set; }

        [DeserializeAs(Name = "access_token")]
        public string Token { get; set; }
    }
}
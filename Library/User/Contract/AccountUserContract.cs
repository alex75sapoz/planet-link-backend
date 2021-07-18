using System.Text.Json.Serialization;

namespace Library.Account.Contract
{
    public class AccountUserContract
    {
        [JsonIgnore]
        public int UserTypeId { get; internal set; }

        public int UserId { get; internal set; }

        public AccountUserGoogleContract? Google { get; internal set; }
        public AccountUserStocktwitsContract? Stocktwits { get; internal set; }

        public AccountUserTypeContract UserType => IAccountMemoryCache.UserTypes[UserTypeId];
    }
}
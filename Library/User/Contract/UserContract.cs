using System.Text.Json.Serialization;

namespace Library.User.Contract
{
    public class UserContract
    {
        [JsonIgnore]
        public int UserTypeId { get; internal set; }

        public int UserId { get; internal set; }

        public UserGoogleContract? Google { get; internal set; }
        public UserStocktwitsContract? Stocktwits { get; internal set; }

        public UserTypeContract Type => IUserMemoryCache.UserTypes[UserTypeId];
    }
}
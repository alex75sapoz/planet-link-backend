namespace Library.User.Contract
{
    public class UserContract
    {
        public int UserId { get; internal set; }

        public UserTypeContract Type { get; internal set; }
        public UserGoogleContract Google { get; internal set; }
        public UserStocktwitsContract Stocktwits { get; internal set; }
    }
}
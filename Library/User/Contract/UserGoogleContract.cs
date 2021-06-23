namespace Library.User.Contract
{
    public class UserGoogleContract
    {
        public UserGoogleContract()
        {
            Name = default!;
            Username = default!;
        }

        public string Name { get; internal set; }
        public string Username { get; internal set; }
    }
}
namespace Library.User.Contract
{
    public class UserTypeContract
    {
        public UserTypeContract()
        {
            Name = default!;
        }

        public int UserTypeId { get; internal set; }
        public string Name { get; internal set; }
    }
}
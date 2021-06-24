namespace Library.User.Contract
{
    public class UserTypeContract
    {
        public int UserTypeId { get; internal set; }
        public string Name { get; internal set; } = default!;
    }
}
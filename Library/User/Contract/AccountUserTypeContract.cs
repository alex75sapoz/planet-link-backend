namespace Library.Account.Contract
{
    public class AccountUserTypeContract
    {
        public int UserTypeId { get; internal set; }
        public string Name { get; internal set; } = default!;
    }
}
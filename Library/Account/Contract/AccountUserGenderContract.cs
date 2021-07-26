namespace Library.Account.Contract
{
    public class AccountUserGenderContract
    {
        public int UserGenderId { get; internal set; }
        public string Name { get; internal set; } = default!;
    }
}
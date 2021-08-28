namespace Library.Account.Enum
{
    public enum AccountDictionary : int
    {
        UserTypes = 1,
        UserGenders = 2,
        Users = 3,
        UserSessions = 4
    }

    public enum UserType : int
    {
        Google = 1,
        Stocktwits = 2,
        Fitbit = 3
    }

    public enum UserGender : int
    {
        Male = 1,
        Female = 2,
        Unknown = 3
    }
}
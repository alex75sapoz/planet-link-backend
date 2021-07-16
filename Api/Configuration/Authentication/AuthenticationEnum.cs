using Library.User.Enum;

namespace Api.Configuration.Authentication
{
    public enum AuthenticationUserType
    {
        Guest = 0,
        Google = UserType.Google,
        Stocktwits = UserType.Stocktwits,
        Fitbit = UserType.Fitbit
    }
}
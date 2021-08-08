namespace Library.Account
{
    static class AccountExtension
    {
        public static int ToUserGenderId(this string input) =>
            System.Enum.TryParse(input, ignoreCase: true, out UserGender userGender)
                ? (int)userGender
                : (int)UserGender.Unknown;
    }
}
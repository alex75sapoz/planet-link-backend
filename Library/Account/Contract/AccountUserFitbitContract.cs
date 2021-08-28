using Newtonsoft.Json;
using System;

namespace Library.Account.Contract
{
    public class AccountUserFitbitContract
    {
        [JsonIgnore]
        public int UserGenderId { get; internal set; }
        public string ShortName => $"{FirstName} {LastName[0]}.";
        public string FullName => $"{FirstName} {LastName}";

        public string FirstName { get; internal set; } = default!;
        public string LastName { get; internal set; } = default!;
        public int AgeInYears { get; internal set; }
        public decimal HeightInCentimeters { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public AccountUserGenderContract UserGender => IAccountMemoryCache.UserGenders[UserGenderId];
    }
}
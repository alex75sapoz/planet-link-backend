using RestSharp.Deserializers;
using System;

namespace Library.Account.Response
{
    class AccountUserFitbitResponseRoot
    {
        public AccountUserFitbitResponse User { get; set; } = default!;
    }

    class AccountUserFitbitResponse
    {
        [DeserializeAs(Name = "encodedId")]
        public string UserId { get; set; } = default!;

        [DeserializeAs(Name = "avatar150")]
        public string AvatarUrl { get; set; } = default!;

        [DeserializeAs(Name = "gender")]
        public string Gender { get; set; } = default!;

        [DeserializeAs(Name = "age")]
        public int AgeInYears { get; set; }

        [DeserializeAs(Name = "firstName")]
        public string FirstName { get; set; } = default!;

        [DeserializeAs(Name = "lastName")]
        public string LastName { get; set; } = default!;

        [DeserializeAs(Name = "height")]
        public decimal HeightInCentimeters { get; set; } = default!;

        //Data provider returns date string without time or offset
        [DeserializeAs(Name = "memberSince")]
        public DateTimeOffset CreatedOn { get; set; }
    }
}
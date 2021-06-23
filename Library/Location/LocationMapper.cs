namespace Library.Location
{
    internal static class LocationMapper
    {
        public static LocationCountryContract MapToCountryContract(this LocationCountryEntity src) => new()
        {
            CountryId = src.CountryId,
            Name = src.Name,
            ThreeLetterCode = src.ThreeLetterCode,
            TwoLetterCode = src.TwoLetterCode
        };

        public static LocationStateContract MapToStateContract(this LocationStateEntity src) => new()
        {
            StateId = src.StateId,
            Name = src.Name,
            TwoLetterCode = src.TwoLetterCode
        };

        public static LocationCityContract MapToCityContract(this LocationCityEntity src) => new()
        {
            OpenWeatherId = src.OpenWeatherId,
            IsCapital = src.CityId == src.Country.CapitalCityId,
            CoordinatesLookup = LocationExtension.GetCoordinatesLookup((src.Latitude, src.Longitude)),
            CountryId = src.CountryId,
            StateId = src.StateId,
            CityId = src.CityId,
            Name = src.Name,
            County = src.County,
            Zipcode = src.Zipcode,
            Latitude = src.Latitude,
            Longitude = src.Longitude
        };
    }
}
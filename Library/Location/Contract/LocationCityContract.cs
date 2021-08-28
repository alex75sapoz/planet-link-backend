using System.Text.Json.Serialization;

namespace Library.Location.Contract
{
    public class LocationCityContract
    {
        [JsonIgnore]
        public int OpenWeatherId { get; internal set; }
        [JsonIgnore]
        public bool IsCapital { get; internal set; }
        [JsonIgnore]
        public string CoordinatesLookup { get; internal set; } = default!;
        [JsonIgnore]
        public int CountryId { get; internal set; }
        [JsonIgnore]
        public int? StateId { get; internal set; }

        public int CityId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string? County { get; internal set; }
        public string? Zipcode { get; internal set; }
        public decimal Latitude { get; internal set; }
        public decimal Longitude { get; internal set; }

        public LocationCountryContract Country => ILocationMemoryCache.Countries[CountryId];
        public LocationStateContract? State => StateId.HasValue ? ILocationMemoryCache.States[StateId.Value] : null;
    }
}
using System.Text.Json.Serialization;

namespace Api.Library.Location.Contract
{
    public class LocationCityContract
    {
        [JsonIgnore]
        public int OpenWeatherId { get; internal set; }
        [JsonIgnore]
        public bool IsCapital { get; internal set; }
        [JsonIgnore]
        public string CoordinatesLookup { get; internal set; }

        public int CityId { get; internal set; }
        public string Name { get; internal set; }
        public string County { get; internal set; }
        public string Zipcode { get; internal set; }
        public decimal Latitude { get; internal set; }
        public decimal Longitude { get; internal set; }

        public LocationCountryContract Country { get; internal set; }
        public LocationStateContract State { get; internal set; }
    }
}
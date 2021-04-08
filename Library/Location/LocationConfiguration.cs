namespace Library.Location
{
    internal class LocationConfiguration
    {
        public LocationLimit Limit { get; set; }
        public LocationDefault Default { get; set; }

        public class LocationLimit
        {
            public int SearchCountriesLimit { get; set; }
            public int SearchCitiesLimit { get; set; }
        }

        public class LocationDefault
        {
            public int SpatialReferenceId { get; set; }
        }
    }
}
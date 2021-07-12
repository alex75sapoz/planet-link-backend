namespace Library.Location
{
    public class LocationConfiguration
    {
        public LocationLimit Limit { get; set; } = default!;
        public LocationDefault Default { get; set; } = default!;

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
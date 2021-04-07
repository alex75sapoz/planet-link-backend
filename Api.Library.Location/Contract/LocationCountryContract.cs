namespace Api.Library.Location.Contract
{
    public class LocationCountryContract
    {
        public int CountryId { get; internal set; }
        public string Name { get; internal set; }
        public string TwoLetterCode { get; internal set; }
        public string ThreeLetterCode { get; internal set; }
    }
}
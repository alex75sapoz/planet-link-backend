namespace Library.Location.Contract
{
    public class LocationCountryContract
    {
        public int CountryId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string TwoLetterCode { get; internal set; } = default!;
        public string ThreeLetterCode { get; internal set; } = default!;
    }
}
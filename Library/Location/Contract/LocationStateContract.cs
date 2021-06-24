namespace Library.Location.Contract
{
    public class LocationStateContract
    {
        public int StateId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string TwoLetterCode { get; internal set; } = default!;
    }
}
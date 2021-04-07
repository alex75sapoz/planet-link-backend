namespace Api.Library.Location
{
    internal static class LocationExtension
    {
        public static string GetCoordinatesLookup((decimal latitude, decimal longitude) coordinates) =>
            $"{coordinates.latitude.ToString().Substring(0, coordinates.latitude < 0 ? 2 : 1)}{coordinates.longitude.ToString().Substring(0, coordinates.longitude < 0 ? 2 : 1)}";
    }
}
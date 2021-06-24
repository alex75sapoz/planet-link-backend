namespace Api.Configuration.Database
{
    class DatabaseConfiguration
    {
        public string Connection { get; set; } = default!;
        public string Server { get; set; } = default!;
        public string Database { get; set; } = default!;
        public string Location { get; set; } = default!;
    }
}
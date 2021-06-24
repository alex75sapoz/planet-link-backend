namespace Api.Configuration.CrossOriginRequest
{
    class CrossOriginRequestConfiguration
    {
        public string Name { get; set; } = default!;
        public string[] Origins { get; set; } = default!;
        public string[] Headers { get; set; } = default!;
        public string[] Methods { get; set; } = default!;
    }
}
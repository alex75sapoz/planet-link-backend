namespace Api.Configuration.CrossOriginRequest
{
    internal class CrossOriginRequestConfiguration
    {
        public string Name { get; set; }
        public string[] Origins { get; set; }
        public string[] Headers { get; set; }
        public string[] Methods { get; set; }
    }
}
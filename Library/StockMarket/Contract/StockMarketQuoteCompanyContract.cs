using System;

namespace Library.StockMarket.Contract
{
    public class StockMarketQuoteCompanyContract
    {
        public string Name { get; internal set; } = default!;
        public string Description { get; internal set; } = default!;
        public string Exchange { get; internal set; } = default!;
        public string? Industry { get; internal set; }
        public string? WebsiteUrl { get; internal set; }
        public string? ChiefExecutiveOfficer { get; internal set; }
        public string? Sector { get; internal set; }
        public string? Country { get; internal set; }
        public int? Employees { get; internal set; }
        public string? Phone { get; internal set; }
        public string? Address { get; internal set; }
        public string? City { get; internal set; }
        public string? State { get; internal set; }
        public string? Zipcode { get; internal set; }
        public string? LogoUrl { get; internal set; }
        public decimal? Beta { get; internal set; }
        public DateTimeOffset? InitialPublicOfferingOn { get; internal set; }
    }
}
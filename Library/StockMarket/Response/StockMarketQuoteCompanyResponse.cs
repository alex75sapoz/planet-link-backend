using RestSharp.Deserializers;
using System;

namespace Library.StockMarket.Response
{
    internal class StockMarketQuoteCompanyResponse
    {
        public string Symbol { get; set; }

        [DeserializeAs(Name = "companyName")]
        public string Name { get; set; }

        public string Description { get; set; }

        [DeserializeAs(Name = "exchangeShortName")]
        public string Exchange { get; set; }

        public string Industry { get; set; }

        [DeserializeAs(Name = "website")]
        public string WebsiteUrl { get; set; }

        [DeserializeAs(Name = "ceo")]
        public string ChiefExecutiveOfficer { get; set; }

        public string Sector { get; set; }

        public string Country { get; set; }

        [DeserializeAs(Name = "fullTimeEmployees")]
        public int? Employees { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        [DeserializeAs(Name = "zip")]
        public string Zipcode { get; set; }

        [DeserializeAs(Name = "image")]
        public string LogoUrl { get; set; }

        public decimal? Beta { get; set; }

        //Data provider returns date string without time or offset
        [DeserializeAs(Name = "ipoDate")]
        public DateTimeOffset? InitialPublicOfferingOn { get => InitialPublicOfferingOnAtEasternTimezone; set => InitialPublicOfferingOnAtEasternTimezone = value?.DateTime.SetTimezone(BaseExtension.EasternTimezone); }
        private DateTimeOffset? InitialPublicOfferingOnAtEasternTimezone { get; set; }
    }
}
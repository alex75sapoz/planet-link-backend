﻿using RestSharp.Deserializers;

namespace Library.StockMarket.Response
{
    internal class StockMarketQuoteResponse
    {
        public string Symbol { get; set; }

        public string Name { get; set; }

        [DeserializeAs(Name = "exchangeShortName")]
        public string Exchange { get; set; }
    }
}
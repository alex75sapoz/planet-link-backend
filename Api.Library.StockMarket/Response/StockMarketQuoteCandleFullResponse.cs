﻿using RestSharp.Deserializers;
using System.Collections.Generic;

namespace Api.Library.StockMarket.Response
{
    internal class StockMarketQuoteCandleFullResponse
    {
        public string Symbol { get; set; }

        [DeserializeAs(Name = "historical")]
        public List<StockMarketQuoteCandleResponse> Candles { get; set; }
    }
}
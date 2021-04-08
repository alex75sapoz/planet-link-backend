﻿using Api.Library.StockMarket.Entity;
using Microsoft.EntityFrameworkCore;

namespace Api.Library.StockMarket
{
    internal class StockMarketContext : LibraryContext
    {
        public StockMarketContext(DbContextOptions<StockMarketContext> options) : base(options) { }

        public DbSet<StockMarketQuoteEntity> StockMarketQuotes { get; set; }
        public DbSet<StockMarketExchangeEntity> StockMarketExchanges { get; set; }
        public DbSet<StockMarketTimeframeEntity> StockMarketTimeframes { get; set; }
        public DbSet<StockMarketQuoteUserAlertEntity> StockMarketQuoteUserAlerts { get; set; }
        public DbSet<StockMarketAlertTypeEntity> StockMarketAlertTypes { get; set; }
        public DbSet<StockMarketAlertCompletedTypeEntity> StockMarketAlertCompletedTypes { get; set; }
        public DbSet<StockMarketQuoteUserEmotionEntity> StockMarketQuoteUserEmotions { get; set; }
        public DbSet<StockMarketEmotionEntity> StockMarketEmotions { get; set; }
    }
}
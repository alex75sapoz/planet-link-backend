using Microsoft.EntityFrameworkCore;

namespace Library.StockMarket
{
    class StockMarketContext : BaseContext
    {
        public StockMarketContext(DbContextOptions<StockMarketContext> options) : base(options) { }

        public DbSet<StockMarketQuoteEntity> Quotes { get; set; } = default!;
        public DbSet<StockMarketExchangeEntity> Exchanges { get; set; } = default!;
        public DbSet<StockMarketTimeframeEntity> Timeframes { get; set; } = default!;
        public DbSet<StockMarketQuoteUserAlertEntity> QuoteUserAlerts { get; set; } = default!;
        public DbSet<StockMarketAlertTypeEntity> AlertTypes { get; set; } = default!;
        public DbSet<StockMarketAlertCompletedTypeEntity> AlertCompletedTypes { get; set; } = default!;
        public DbSet<StockMarketQuoteUserEmotionEntity> QuoteUserEmotions { get; set; } = default!;
        public DbSet<StockMarketEmotionEntity> Emotions { get; set; } = default!;
    }
}
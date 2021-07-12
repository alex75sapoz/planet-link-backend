using Microsoft.EntityFrameworkCore;

namespace Library.StockMarket
{
    class StockMarketContext : BaseContext
    {
        public StockMarketContext(DbContextOptions<StockMarketContext> options) : base(options) { }

        public DbSet<StockMarketQuoteEntity> StockMarketQuotes { get; set; } = default!;
        public DbSet<StockMarketExchangeEntity> StockMarketExchanges { get; set; } = default!;
        public DbSet<StockMarketTimeframeEntity> StockMarketTimeframes { get; set; } = default!;
        public DbSet<StockMarketQuoteUserAlertEntity> StockMarketQuoteUserAlerts { get; set; } = default!;
        public DbSet<StockMarketAlertTypeEntity> StockMarketAlertTypes { get; set; } = default!;
        public DbSet<StockMarketAlertCompletedTypeEntity> StockMarketAlertCompletedTypes { get; set; } = default!;
        public DbSet<StockMarketQuoteUserEmotionEntity> StockMarketQuoteUserEmotions { get; set; } = default!;
        public DbSet<StockMarketEmotionEntity> StockMarketEmotions { get; set; } = default!;
    }
}
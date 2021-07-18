using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.StockMarket.Entity
{
    public class StockMarketTimeframeEntity
    {
        public int TimeframeId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public int Prefix { get; internal set; }
        public string Suffix { get; internal set; } = default!;
        public int Multiplier { get; internal set; }
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    class StockMarketTimeframeEntityConfiguration : IEntityTypeConfiguration<StockMarketTimeframeEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketTimeframeEntity> entity)
        {
            entity.ToTable(nameof(StockMarket) + nameof(StockMarketContext.Timeframes));
            entity.HasKey(timeframe => timeframe.TimeframeId);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.StockMarket.Entity
{
    public class StockMarketTimeframeEntity
    {
        public int TimeframeId { get; internal set; }
        public string Name { get; internal set; }
        public int Prefix { get; internal set; }
        public string Suffix { get; internal set; }
        public int Multiplier { get; internal set; }
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    internal class StockMarketTimeframeEntityConfiguration : IEntityTypeConfiguration<StockMarketTimeframeEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketTimeframeEntity> entity)
        {
            entity.ToTable(nameof(StockMarketContext.StockMarketTimeframes));
            entity.HasKey(timeframe => timeframe.TimeframeId);
        }
    }
}
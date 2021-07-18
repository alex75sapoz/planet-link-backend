using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.StockMarket.Entity
{
    public class StockMarketExchangeEntity
    {
        public int ExchangeId { get; internal set; }
        public string FinancialModelingPrepId { get; internal set; } = default!;
        public string Name { get; internal set; } = default!;

        public virtual ICollection<StockMarketQuoteEntity> Quotes { get; internal set; } = new HashSet<StockMarketQuoteEntity>();
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    class StockMarketExchangeEntityConfiguration : IEntityTypeConfiguration<StockMarketExchangeEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketExchangeEntity> entity)
        {
            entity.ToTable(nameof(StockMarket) + nameof(StockMarketContext.Exchanges));
            entity.HasKey(exchange => exchange.ExchangeId);

            entity.HasMany(exchange => exchange.Quotes).WithOne(quote => quote.Exchange);
        }
    }
}
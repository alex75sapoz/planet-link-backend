using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.StockMarket.Entity
{
    public class StockMarketExchangeEntity
    {
        public int ExchangeId { get; internal set; }
        public string FinancialModelingPrepId { get; internal set; }
        public string Name { get; internal set; }

        public virtual ICollection<StockMarketQuoteEntity> Quotes { get; internal set; }
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    internal class StockMarketExchangeEntityConfiguration : IEntityTypeConfiguration<StockMarketExchangeEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketExchangeEntity> entity)
        {
            entity.ToTable(nameof(StockMarketContext.StockMarketExchanges));
            entity.HasKey(exchange => exchange.ExchangeId);

            entity.HasMany(exchange => exchange.Quotes).WithOne(quote => quote.Exchange);
        }
    }
}
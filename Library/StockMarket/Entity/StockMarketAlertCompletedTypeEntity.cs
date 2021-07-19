using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.StockMarket.Entity
{
    public class StockMarketAlertCompletedTypeEntity
    {
        public int AlertCompletedTypeId { get; internal set; }
        public string Name { get; internal set; } = default!;

        public virtual ICollection<StockMarketQuoteUserAlertEntity> QuoteUserAlerts { get; internal set; } = new HashSet<StockMarketQuoteUserAlertEntity>();
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    class StockMarketAlertCompletedTypeEntityConfiguration : IEntityTypeConfiguration<StockMarketAlertCompletedTypeEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketAlertCompletedTypeEntity> entity)
        {
            entity.ToTable(nameof(StockMarket) + nameof(StockMarketContext.AlertCompletedTypes));
            entity.HasKey(alertCompletedType => alertCompletedType.AlertCompletedTypeId);

            entity.HasMany(alertCompletedType => alertCompletedType.QuoteUserAlerts).WithOne(quoteUserAlert => quoteUserAlert.AlertCompletedType!);
        }
    }
}
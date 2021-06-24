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
            entity.ToTable(nameof(StockMarketContext.StockMarketAlertCompletedTypes));
            entity.HasKey(completedType => completedType.AlertCompletedTypeId);

            entity.HasMany(completedType => completedType.QuoteUserAlerts).WithOne(quoteUserAlert => quoteUserAlert.CompletedType!).HasForeignKey(quoteUserAlert => quoteUserAlert.AlertCompletedTypeId);
        }
    }
}
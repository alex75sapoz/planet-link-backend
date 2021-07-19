using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.StockMarket.Entity
{
    public class StockMarketAlertTypeEntity
    {
        public int AlertTypeId { get; internal set; }
        public string Name { get; internal set; } = default!;

        public virtual ICollection<StockMarketQuoteUserAlertEntity> QuoteUserAlerts { get; internal set; } = new HashSet<StockMarketQuoteUserAlertEntity>();
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    class StockMarketAlertTypeEntityConfiguration : IEntityTypeConfiguration<StockMarketAlertTypeEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketAlertTypeEntity> entity)
        {
            entity.ToTable(nameof(StockMarket) + nameof(StockMarketContext.AlertTypes));
            entity.HasKey(alertType => alertType.AlertTypeId);

            entity.HasMany(alertType => alertType.QuoteUserAlerts).WithOne(quoteUserAlert => quoteUserAlert.AlertType);
        }
    }
}
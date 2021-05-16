﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.StockMarket.Entity
{
    public class StockMarketAlertTypeEntity
    {
        public StockMarketAlertTypeEntity()
        {
            QuoteUserAlerts = new HashSet<StockMarketQuoteUserAlertEntity>();
        }

        public int AlertTypeId { get; internal set; }
        public string Name { get; internal set; }

        public virtual ICollection<StockMarketQuoteUserAlertEntity> QuoteUserAlerts { get; internal set; }
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    internal class StockMarketAlertTypeEntityConfiguration : IEntityTypeConfiguration<StockMarketAlertTypeEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketAlertTypeEntity> entity)
        {
            entity.ToTable(nameof(StockMarketContext.StockMarketAlertTypes));
            entity.HasKey(alertType => alertType.AlertTypeId);

            entity.HasMany(alertType => alertType.QuoteUserAlerts).WithOne(quoteUserAlert => quoteUserAlert.AlertType);
        }
    }
}
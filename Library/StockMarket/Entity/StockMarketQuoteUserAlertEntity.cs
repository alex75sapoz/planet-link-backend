using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Library.StockMarket.Entity
{
    public class StockMarketQuoteUserAlertEntity
    {
        public int QuoteUserAlertId { get; internal set; }
        public int QuoteId { get; internal set; }
        public int UserId { get; internal set; }
        public int AlertTypeId { get; internal set; }
        public decimal Buy { get; internal set; }
        public decimal Sell { get; internal set; }
        public decimal StopLoss { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
        public DateTimeOffset LastCheckOn { get; internal set; }
        public DateTimeOffset LastReverseSplitCheckOn { get; internal set; }
        public int ReverseSplitCount { get; internal set; }
        public int? AlertCompletedTypeId { get; internal set; }
        public decimal? CompletedSell { get; internal set; }
        public DateTimeOffset? CompletedOn { get; internal set; }

        public virtual StockMarketQuoteEntity Quote { get; internal set; } = default!;
        public virtual StockMarketAlertTypeEntity Type { get; internal set; } = default!;
        public virtual StockMarketAlertCompletedTypeEntity? CompletedType { get; internal set; }
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    class UserStockMrketAlertEntityConfiguration : IEntityTypeConfiguration<StockMarketQuoteUserAlertEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketQuoteUserAlertEntity> entity)
        {
            entity.ToTable(nameof(StockMarketContext.StockMarketQuoteUserAlerts));
            entity.HasKey(quoteUserAlert => quoteUserAlert.QuoteUserAlertId);

            entity.Property(quoteUserAlert => quoteUserAlert.Buy).HasPrecision(18, 6);
            entity.Property(quoteUserAlert => quoteUserAlert.Sell).HasPrecision(18, 6);
            entity.Property(quoteUserAlert => quoteUserAlert.StopLoss).HasPrecision(18, 6);
            entity.Property(quoteUserAlert => quoteUserAlert.CompletedSell).HasPrecision(18, 6);

            entity.HasOne(quoteUserAlert => quoteUserAlert.Quote).WithMany(quote => quote.UserAlerts).IsRequired(true);
            entity.HasOne(quoteUserAlert => quoteUserAlert.Type).WithMany(type => type.QuoteUserAlerts).HasForeignKey(quoteUserAlert => quoteUserAlert.AlertTypeId).IsRequired(true);
            entity.HasOne(quoteUserAlert => quoteUserAlert.CompletedType).WithMany(completedType => completedType!.QuoteUserAlerts).HasForeignKey(quoteUserAlert => quoteUserAlert.AlertCompletedTypeId).IsRequired(false);
        }
    }
}
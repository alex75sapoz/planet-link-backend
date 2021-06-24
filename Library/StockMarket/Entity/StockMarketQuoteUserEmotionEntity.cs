using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Library.StockMarket.Entity
{
    public class StockMarketQuoteUserEmotionEntity
    {
        public int QuoteUserEmotionId { get; internal set; }
        public int QuoteId { get; internal set; }
        public int UserId { get; internal set; }
        public int EmotionId { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public virtual StockMarketEmotionEntity Emotion { get; internal set; } = default!;
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    class StockMarketQuoteUserEmotionEntityConfiguration : IEntityTypeConfiguration<StockMarketQuoteUserEmotionEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketQuoteUserEmotionEntity> entity)
        {
            entity.ToTable(nameof(StockMarketContext.StockMarketQuoteUserEmotions));
            entity.HasKey(quoteUserEmotion => quoteUserEmotion.QuoteUserEmotionId);

            entity.HasOne(quoteUserEmotion => quoteUserEmotion.Emotion).WithMany(emotion => emotion.QuoteUserEmotions).IsRequired(true);
        }
    }
}
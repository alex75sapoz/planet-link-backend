using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.StockMarket.Entity
{
    public class StockMarketEmotionEntity
    {
        public int EmotionId { get; internal set; }
        public string Name { get; internal set; }
        public string Emoji { get; internal set; }

        public virtual ICollection<StockMarketQuoteUserEmotionEntity> QuoteUserEmotions { get; internal set; }
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    internal class StockMarketEmotionEntityConfiguration : IEntityTypeConfiguration<StockMarketEmotionEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketEmotionEntity> entity)
        {
            entity.ToTable(nameof(StockMarketContext.StockMarketEmotions));
            entity.HasKey(emotion => emotion.EmotionId);

            entity.HasMany(emotion => emotion.QuoteUserEmotions).WithOne(quoteUserEmotion => quoteUserEmotion.Emotion);
        }
    }
}
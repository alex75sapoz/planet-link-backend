using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.StockMarket.Entity
{
    public class StockMarketEmotionEntity
    {
        public int EmotionId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string Emoji { get; internal set; } = default!;

        public virtual ICollection<StockMarketQuoteUserEmotionEntity> QuoteUserEmotions { get; internal set; } = new HashSet<StockMarketQuoteUserEmotionEntity>();
    }
}

namespace Library.StockMarket.Entity.Configuration
{
    class StockMarketEmotionEntityConfiguration : IEntityTypeConfiguration<StockMarketEmotionEntity>
    {
        public void Configure(EntityTypeBuilder<StockMarketEmotionEntity> entity)
        {
            entity.ToTable(nameof(StockMarket) + nameof(StockMarketContext.Emotions));
            entity.HasKey(emotion => emotion.EmotionId);

            entity.HasMany(emotion => emotion.QuoteUserEmotions).WithOne(quoteUserEmotion => quoteUserEmotion.Emotion);
        }
    }
}
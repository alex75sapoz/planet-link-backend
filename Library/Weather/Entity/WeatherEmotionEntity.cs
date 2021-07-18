using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Weather.Entity
{
    public class WeatherEmotionEntity
    {
        public int EmotionId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string Emoji { get; internal set; } = default!;

        public virtual ICollection<WeatherCityUserEmotionEntity> CityUserEmotions { get; internal set; } = new HashSet<WeatherCityUserEmotionEntity>();
    }
}

namespace Library.Weather.Entity.Configuration
{
    class WeatherEmotionEntityConfiguration : IEntityTypeConfiguration<WeatherEmotionEntity>
    {
        public void Configure(EntityTypeBuilder<WeatherEmotionEntity> entity)
        {
            entity.ToTable(nameof(Weather) + nameof(WeatherContext.Emotions));
            entity.HasKey(emotion => emotion.EmotionId);

            entity.HasMany(emotion => emotion.CityUserEmotions).WithOne(cityUserEmotion => cityUserEmotion.Emotion);
        }
    }
}
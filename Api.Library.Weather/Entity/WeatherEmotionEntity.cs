using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Api.Library.Weather.Entity
{
    public class WeatherEmotionEntity
    {
        public int EmotionId { get; internal set; }
        public string Name { get; internal set; }
        public string Emoji { get; internal set; }

        public virtual ICollection<WeatherCityUserEmotionEntity> CityUserEmotions { get; internal set; }
    }
}

namespace Api.Library.Weather.Entity.Configuration
{
    internal class WeatherEmotionEntityConfiguration : IEntityTypeConfiguration<WeatherEmotionEntity>
    {
        public void Configure(EntityTypeBuilder<WeatherEmotionEntity> entity)
        {
            entity.ToTable(nameof(WeatherContext.WeatherEmotions));
            entity.HasKey(emotion => emotion.EmotionId);

            entity.HasMany(emotion => emotion.CityUserEmotions).WithOne(cityUserEmotion => cityUserEmotion.Emotion);
        }
    }
}
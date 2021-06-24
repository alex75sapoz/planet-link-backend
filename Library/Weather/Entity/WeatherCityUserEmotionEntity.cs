using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Library.Weather.Entity
{
    public class WeatherCityUserEmotionEntity
    {
        public int CityUserEmotionId { get; internal set; }
        public int CityId { get; internal set; }
        public int UserId { get; internal set; }
        public int EmotionId { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public virtual WeatherEmotionEntity Emotion { get; internal set; } = default!;
    }
}

namespace Library.Weather.Entity.Configuration
{
    class WeatherCityUserEmotionEntityConfiguration : IEntityTypeConfiguration<WeatherCityUserEmotionEntity>
    {
        public void Configure(EntityTypeBuilder<WeatherCityUserEmotionEntity> entity)
        {
            entity.ToTable(nameof(WeatherContext.WeatherCityUserEmotions));
            entity.HasKey(cityUserEmotion => cityUserEmotion.CityUserEmotionId);

            entity.HasOne(cityUserEmotion => cityUserEmotion.Emotion).WithMany(emotion => emotion.CityUserEmotions).IsRequired(true);
        }
    }
}
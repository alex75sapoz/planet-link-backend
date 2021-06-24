using Microsoft.EntityFrameworkCore;

namespace Library.Location
{
    class LocationContext : BaseContext
    {
        public LocationContext(DbContextOptions<LocationContext> options) : base(options) { }

        public DbSet<LocationCityEntity> LocationCities { get; set; } = default!;
        public DbSet<LocationContinentEntity> LocationContinents { get; set; } = default!;
        public DbSet<LocationAreaCodeEntity> LocationAreaCodes { get; set; } = default!;
        public DbSet<LocationCurrencyEntity> LocationCurrencies { get; set; } = default!;
        public DbSet<LocationLanguageEntity> LocationLanguages { get; set; } = default!;
        public DbSet<LocationCountryAreaCodeEntity> LocationCountryAreaCodes { get; set; } = default!;
        public DbSet<LocationCountryCurrencyEntity> LocationCountryCurrencies { get; set; } = default!;
        public DbSet<LocationCountryEntity> LocationCountries { get; set; } = default!;
        public DbSet<LocationCountryLanguageEntity> LocationCountryLanguages { get; set; } = default!;
        public DbSet<LocationStateEntity> LocationStates { get; set; } = default!;
        public DbSet<LocationSubContinentEntity> LocationSubContinents { get; set; } = default!;
    }
}
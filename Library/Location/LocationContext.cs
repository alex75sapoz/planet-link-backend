using Microsoft.EntityFrameworkCore;

namespace Library.Location
{
    class LocationContext : BaseContext
    {
        public LocationContext(DbContextOptions<LocationContext> options) : base(options) { }

        public DbSet<LocationCityEntity> Cities { get; set; } = default!;
        public DbSet<LocationContinentEntity> Continents { get; set; } = default!;
        public DbSet<LocationAreaCodeEntity> AreaCodes { get; set; } = default!;
        public DbSet<LocationCurrencyEntity> Currencies { get; set; } = default!;
        public DbSet<LocationLanguageEntity> Languages { get; set; } = default!;
        public DbSet<LocationCountryAreaCodeEntity> CountryAreaCodes { get; set; } = default!;
        public DbSet<LocationCountryCurrencyEntity> CountryCurrencies { get; set; } = default!;
        public DbSet<LocationCountryEntity> Countries { get; set; } = default!;
        public DbSet<LocationCountryLanguageEntity> CountryLanguages { get; set; } = default!;
        public DbSet<LocationStateEntity> States { get; set; } = default!;
        public DbSet<LocationSubContinentEntity> SubContinents { get; set; } = default!;
    }
}
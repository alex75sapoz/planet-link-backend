using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Location
{
    class LocationRepository : BaseRepository<LocationContext>, ILocationRepository
    {
        public LocationRepository(LocationContext context) : base(context) { }

        public async Task<List<LocationCountryEntity>> GetCountriesAsync() =>
            await _context.Countries
                .Include(country => country.Continent)
                .Include(country => country.SubContinent)
                .ToListAsync();

        public async Task<LocationCountryEntity?> GetCountryAsync(int countryId) =>
            await _context.Countries
                .Include(country => country.Continent)
                .Include(country => country.SubContinent)
                .SingleOrDefaultAsync(country => country.CountryId == countryId);

        public async Task<List<LocationCityEntity>> GetCitiesAsync() =>
            await _context.Cities
                .Include(city => city.State)
                .Include(city => city.Country)
                .ToListAsync();

        public async Task<LocationCityEntity?> GetCityAsync(int cityId) =>
            await _context.Cities
                .Include(city => city.State)
                .Include(city => city.Country)
                .SingleOrDefaultAsync(city => city.CityId == cityId);

        public async Task<List<LocationStateEntity>> GetStatesAsync() =>
            await _context.States
                .ToListAsync();

        public async Task<LocationStateEntity?> GetStateAsync(int stateId) =>
            await _context.States
                .FindAsync(stateId);
    }

    public interface ILocationRepository
    {
        Task<List<LocationCityEntity>> GetCitiesAsync();
        Task<LocationCityEntity?> GetCityAsync(int cityId);
        Task<List<LocationCountryEntity>> GetCountriesAsync();
        Task<LocationCountryEntity?> GetCountryAsync(int countryId);
        Task<List<LocationStateEntity>> GetStatesAsync();
        Task<LocationStateEntity?> GetStateAsync(int stateId);
    }
}
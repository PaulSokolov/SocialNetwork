using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface ICityRepository : IRepository<City>
    {
        IQueryable<City> GetAll();
        City GetCity(long id);
        IQueryable<City> GetAllCitiesByCountryId(long countryId);
        Task<IQueryable<City>> GetAllAsync();
        Task<City> GetCityAsync(long id);
        Task<IQueryable<City>> GetAllCitiesByCountryIdAsync(long countryId);
    }
}

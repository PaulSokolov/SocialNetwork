using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface ICityRepository : IRepository<City>
    {
        IQueryable<City> GetAll();
        City GetCity(long id);
        Task<City> GetCityAsync(long id);
        IQueryable<City> GetAllCitiesByCountryId(long countryId);

    }
}

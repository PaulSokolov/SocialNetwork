using DataLayer.Entities;
using System.Linq;

namespace DataLayer.Interfaces
{
    public interface ICityRepository : IRepository<City>
    {
        IQueryable<City> GetAll();
        City GetCity(long id);
        IQueryable<City> GetAllCitiesByCountryId(long countryId);
    }
}

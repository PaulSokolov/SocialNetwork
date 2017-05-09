using System.Linq;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface ICityRepository : IRepository<City>
    {
        IQueryable<City> GetAll();
        City GetCity(long id);
        IQueryable<City> GetAllCitiesByCountryId(long countryId);
    }
}

using System.Linq;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        IQueryable<Country> GetAll();
        Country GetCountry(long id);
    }
}

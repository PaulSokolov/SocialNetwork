using DataLayer.Entities;
using System.Linq;

namespace DataLayer.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        IQueryable<Country> GetAll();
        Country GetCountry(long id);
    }
}

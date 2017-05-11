using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        IQueryable<Country> GetAll();
        Country GetCountry(long id);
        Task<IQueryable<Country>> GetAllAsync();
        Task<Country> GetCountryAsync(long id);
    }
}

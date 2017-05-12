using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface ICityRepository : IRepository<City, long>
    {
    }
}

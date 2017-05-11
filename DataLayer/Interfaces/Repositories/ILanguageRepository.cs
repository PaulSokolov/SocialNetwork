using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface ILanguageRepository : IRepository<Language>
    {
        IQueryable<Language> GetAll();
        Language GetLanguage(long id);
        Task<Language> GetLanguageAsync(long id);
    }
}

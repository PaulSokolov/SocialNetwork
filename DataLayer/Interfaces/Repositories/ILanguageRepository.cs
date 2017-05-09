using System.Linq;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface ILanguageRepository : IRepository<Language>
    {
        IQueryable<Language> GetAll();
        Language GetLanguage(long id);
    }
}

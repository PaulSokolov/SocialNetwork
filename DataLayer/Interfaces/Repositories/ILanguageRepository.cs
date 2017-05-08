using DataLayer.Entities;
using System.Linq;

namespace DataLayer.Interfaces
{
    public interface ILanguageRepository : IRepository<Language>
    {
        IQueryable<Language> GetAll();
        Language GetLanguage(long id);
    }
}

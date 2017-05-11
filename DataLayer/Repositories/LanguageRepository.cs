using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

namespace DataLayer.Repository
{
    public class LanguageRepository : LocalizationRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(LocalizationContext context) : base(context)
        {
        }

        public IQueryable<Language> GetAll()
        {
            try
            {
                return Context.Languages;
            }
            catch (Exception ex)
            {
                throw new Exception($"GetAll() Failed {ex}");
            }
        }

        public Language GetLanguage(long id)
        {
            try
            {
                return Context.Languages.Find(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"GetLanguage() Failed {ex}");
            }
        }

        public async Task<Language> GetLanguageAsync(long id)
        {
            try
            {
                return await Context.Languages.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"GetLanguageAsync() Failed {ex}");
            }
        }
    }
}

using System;
using System.Linq;
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
    }
}

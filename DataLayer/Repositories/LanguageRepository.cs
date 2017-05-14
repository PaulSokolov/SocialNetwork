using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

namespace DataLayer.Repository
{
    public class LanguageRepository : ILanguageRepository
    {
        private DbContext Context { get; }

        public LanguageRepository(LocalizationContext context)
        {
            Context = context;
        }

        public LanguageRepository(UserProfileContext context)
        {
            Context = context;
        }
        public void Dispose()
        {
            Context.Dispose();
        }

        public Language Get(long id)
        {
            if (id == null)
                throw new ArgumentOutOfRangeException(nameof(id));

            Language entity = null;

            try
            {
                var context = Context as LocalizationContext;
                entity = context != null ? context.Languages.Find(id) : ((UserProfileContext) Context).Languages.Find(id);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("db.Set<{2}>().Find({0}) threw exception: {1}", id, ex, typeof(Language).Name));
            }

            if (entity == null)
                throw new InvalidOperationException($"{typeof(Language).Name} with ID={id} was not found in the DB");

            return entity;
        }

        public async Task<Language> GetAsync(long id)
        {
            if (id == null)
                throw new ArgumentOutOfRangeException(nameof(id));

            Language entity = null;

            try
            {
                var context = Context as LocalizationContext;
                entity = context != null ? await context.Languages.FindAsync(id) : await ((UserProfileContext)Context).Languages.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("db.Set<{2}>().FindAsync({0}) threw exception: {1}", id, ex, typeof(Language).Name));

            }

            if (entity == null)
                throw new InvalidOperationException($"{typeof(Language).Name} with ID={id} was not found in the DB");

            return entity;
        }

        public IQueryable<Language> GetAll()
        {
            var context = Context as LocalizationContext;
            return context != null ? context.Languages : ((UserProfileContext) Context).Languages;
        }

        public Language Add(Language entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var context = Context as LocalizationContext;
            return context != null ? context.Languages.Add(entity) : ((UserProfileContext)Context).Languages.Add(entity);
        }

        public async Task<Language> AddAsync(Language entity)
        {
            Task<Language> task = new Task<Language>(() => Add(entity));
            task.Start();
            return await task;
        }

        public Language Update(Language entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (Context is LocalizationContext)
            {
                ((LocalizationContext)Context).Languages.Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
                return entity;
            }

            ((UserProfileContext)Context).Languages.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public async Task<Language> UpdateAsync(Language entity)
        {
            var task = new Task<Language>(() => Update(entity));
            task.Start();
            return await task;
        }

        public Language Delete(Language entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var context = Context as LocalizationContext;
            return context != null ? context.Languages.Remove(entity) : ((UserProfileContext)Context).Languages.Remove(entity);
        }

        public async Task<Language> DeleteAsync(Language entity)
        {
            var task = new Task<Language>(() => Delete(entity));
            task.Start();
            return await task;
        }
    }
}

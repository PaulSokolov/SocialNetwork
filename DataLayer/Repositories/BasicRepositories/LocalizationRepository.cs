using System;
using System.Data.Entity;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

namespace DataLayer.BasicRepositories
{
    public abstract class LocalizationRepository<T> : IRepository<T> where T : Entity
    {
        protected LocalizationRepository(LocalizationContext context)
        {
            Context = context;
        }
        public void Dispose()
        {
        }

        public T Get(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            T entity = null;

            try
            {
                entity = GetEntity(id);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("db.Set<{2}>().Find({0}) threw exception: {1}", id, ex, typeof(T).Name));

            }

            if (entity == null)
                throw new InvalidOperationException($"{typeof(T).Name} with ID={id} was not found in the DB");

            return entity;
        }

        public T Add(T entity)
        {
            return Context.Set<T>().Add(entity);
        }
        protected LocalizationContext Context { get; }

        private T GetEntity(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public T Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            Context.Set<T>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();

            return entity;
        }
        public T Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return Context.Set<T>().Remove(entity);
        }
    }
}

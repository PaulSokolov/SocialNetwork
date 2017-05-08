using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;
using System;
using System.Data.Entity;

namespace DataLayer.BasicRepositories
{
    public abstract class LocalizationRepository<T> : IRepository<T> where T : Entity
    {
        private readonly LocalizationContext _context;

        public LocalizationRepository(LocalizationContext context)
        {
            _context = context;
        }
        public void Dispose()
        {
        }

        public T Get(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id");

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
                throw new InvalidOperationException(string.Format("{0} with ID={1} was not found in the DB", typeof(T).Name, id));

            return entity;
        }

        public T Add(T entity)
        {
            return _context.Set<T>().Add(entity);
        }
        protected LocalizationContext Context
        {
            get { return _context; }
        }

        private T GetEntity(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public T Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();

            return entity;
        }
        public T Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            return _context.Set<T>().Remove(entity);
        }
    }
}

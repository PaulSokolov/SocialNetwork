using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;
using System;
using System.Data.Entity;

namespace DataLayer.BasicRepositories
{
    public abstract class UserInfoRepository<T> : IRepository<T> where T : Entity
    {
        private readonly UserProfileContext _context;

        public UserInfoRepository(UserProfileContext context)
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
            if (entity == null)
                throw new ArgumentNullException("entity");
            return _context.Set<T>().Add(entity);
        }
        protected UserProfileContext Context
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

            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public T Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _context.Entry(entity).State = EntityState.Deleted;
            return entity;
        }
    }
}

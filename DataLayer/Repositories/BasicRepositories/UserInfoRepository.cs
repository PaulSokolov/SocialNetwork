using System;
using System.Data.Entity;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

namespace DataLayer.BasicRepositories
{
    public abstract class UserInfoRepository<T> : IRepository<T> where T : Entity
    {
        protected UserInfoRepository(UserProfileContext context)
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
                throw new Exception($"db.Set<{typeof(T).Name}>().Find({id}) threw exception: {ex}");
                
            }

            if (entity == null)
                throw new InvalidOperationException($"{typeof(T).Name} with ID={id} was not found in the DB");

            return entity;
        }

        public T Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return Context.Set<T>().Add(entity);
        }
        protected UserProfileContext Context { get; }

        private T GetEntity(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public T Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public T Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Context.Entry(entity).State = EntityState.Deleted;
            return entity;
        }
    }
}

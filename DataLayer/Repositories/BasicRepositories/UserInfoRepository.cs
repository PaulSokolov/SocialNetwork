using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

namespace DataLayer.BasicRepositories
{
    public abstract class UserInfoRepository<T, TKey> : IRepository<T, TKey> where T : Entity
        where TKey:IComparable<TKey>
    {
        protected UserProfileContext Context { get; }

        protected UserInfoRepository(UserProfileContext context)
        {
            Context = context;
        }
        public void Dispose()
        {
            Context.Dispose();
        }

        public T Get(TKey id)
        {
            if (id == null)
                throw new ArgumentOutOfRangeException(nameof(id));

            T entity = null;

            try
            {
                entity = Context.Set<T>().Find(id);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("db.Set<{2}>().Find({0}) threw exception: {1}", id, ex, typeof(T).Name));
            }

            if (entity == null)
                throw new InvalidOperationException($"{typeof(T).Name} with ID={id} was not found in the DB");

            return entity;
        }

        public async Task<T> GetAsync(TKey id)
        {
            if (id == null)
                throw new ArgumentOutOfRangeException(nameof(id));

            T entity = null;

            try
            {
                entity = await Context.Set<T>().FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("db.Set<{2}>().FindAsync({0}) threw exception: {1}", id, ex, typeof(T).Name));

            }

            if (entity == null)
                throw new InvalidOperationException($"{typeof(T).Name} with ID={id} was not found in the DB");

            return entity;
        }

        public IQueryable<T> GetAll()
        {
            return Context.Set<T>();
        }

        public T Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return Context.Set<T>().Add(entity);
        }

        public async Task<T> AddAsync(T entity)
        {
            Task<T> task = new Task<T>(() => Add(entity));
            task.Start();
            return await task;
        }

        public T Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Context.Set<T>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var task = new Task<T>(() => Update(entity));
            task.Start();
            return await task;
        }

        public T Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Context.Set<T>().Remove(entity);
        }

        public async Task<T> DeleteAsync(T entity)
        {
            var task = new Task<T>(() => Delete(entity));
            task.Start();
            return await task;
        }
        //public T Get(int id)
        //{
        //    if (id <= 0)
        //        throw new ArgumentOutOfRangeException(nameof(id));

        //    T entity = null;

        //    try
        //    {
        //        entity = GetEntity(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"db.Set<{typeof(T).Name}>().Find({id}) threw exception: {ex}");

        //    }

        //    if (entity == null)
        //        throw new InvalidOperationException($"{typeof(T).Name} with ID={id} was not found in the DB");

        //    return entity;
        //}

        //public async Task<T> GetAsync(int id)
        //{
        //    if (id <= 0)
        //        throw new ArgumentOutOfRangeException(nameof(id));

        //    T entity = null;

        //    try
        //    {
        //        entity = await GetEntityAsync(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"db.Set<{typeof(T).Name}>().FindAsync({id}) threw exception: {ex}");

        //    }

        //    if (entity == null)
        //        throw new InvalidOperationException($"{typeof(T).Name} with ID={id} was not found in the DB");

        //    return entity;
        //}

        //public T Add(T entity)
        //{
        //    if (entity == null)
        //        throw new ArgumentNullException(nameof(entity));
        //    return Context.Set<T>().Add(entity);
        //}

        //public async Task<T> AddAsync(T entity)
        //{
        //    Task<T> task = new Task<T>(() => Add(entity));
        //    task.Start();
        //    return await task;
        //}


        //private T GetEntity(int id)
        //{
        //    return Context.Set<T>().Find(id);
        //}

        //private async Task<T> GetEntityAsync(int id)
        //{
        //    return await Context.Set<T>().FindAsync(id);
        //}

        //public T Update(T entity)
        //{
        //    if (entity == null)
        //        throw new ArgumentNullException(nameof(entity));

        //    Context.Entry(entity).State = EntityState.Modified;
        //    return entity;
        //}

        //public async Task<T> UpdateAsync(T entity)
        //{
        //    var task = new Task<T>(()=> Update(entity));
        //    task.Start();

        //    return await task;
        //}

        //public T Delete(T entity)
        //{
        //    if (entity == null)
        //        throw new ArgumentNullException(nameof(entity));

        //    Context.Entry(entity).State = EntityState.Deleted;
        //    return entity;
        //}

        //public async Task<T> DeleteAsync(T entity)
        //{
        //    var task = new Task<T>(() => Delete(entity));
        //    task.Start();
        //    return await task;
        //}
    }
}

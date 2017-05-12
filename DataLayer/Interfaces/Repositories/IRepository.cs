using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IRepository<T, TKey> : IDisposable where T : class
        where TKey : IComparable<TKey>
    {
        T Get(TKey id);
        IQueryable<T> GetAll();
        T Add(T entity);
        T Update(T entity);
        T Delete(T entity);
        Task<T> GetAsync(TKey id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}

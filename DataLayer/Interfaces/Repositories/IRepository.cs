using System;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IRepository<T> : IDisposable
    {
        T Get(int id);
        T Add(T entity);
        T Update(T entity);
        T Delete(T entity);
        Task<T> GetAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}

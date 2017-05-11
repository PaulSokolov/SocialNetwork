using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface ITransaction
    {
        void Commit();
        Task CommitAsync();
        T Remove<T>(T entity) where T : Entity;
        Task<T> RemoveAsync<T>(T entity) where T : Entity;
        IEnumerable<T> RemoveRange<T>(IEnumerable<T> entities) where T : Entity;
        Task<IEnumerable<T>> RemoveRangeAsync<T>(IEnumerable<T> entities) where T : Entity;
        T Add<T>(T entity) where T : Entity;
        Task<T> AddAsync<T>(T entity) where T : Entity;
        IEnumerable<T> AddRange<T>(IEnumerable<T> entities) where T : Entity;
        Task<IEnumerable<T>> AddRangeAsync<T>(IEnumerable<T> entities) where T : Entity;
    }
}

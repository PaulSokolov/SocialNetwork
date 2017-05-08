using DataLayer.Entities;
using System.Collections.Generic;

namespace DataLayer.Interfaces
{
    public interface ITransaction
    {
        void Commit();
        void Remove<T>(T entity) where T : Entity;
        void RemoveRange<T>(IEnumerable<T> entities) where T : Entity;
        T Add<T>(T entity) where T : Entity;
        IEnumerable<T> AddRange<T>(IEnumerable<T> entities) where T : Entity;
    }
}

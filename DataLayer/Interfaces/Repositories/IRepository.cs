using System;

namespace DataLayer.Interfaces
{
    public interface IRepository<T> : IDisposable
    {
        T Get(int id);
        T Add(T entity);
        T Update(T entity);
        T Delete(T entity);
    }
}

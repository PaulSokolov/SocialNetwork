using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

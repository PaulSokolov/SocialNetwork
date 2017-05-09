using System;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IClientManager : IDisposable
    {
        UserProfile Create(UserProfile item);
        UserProfile Delete(string id);
    }
}


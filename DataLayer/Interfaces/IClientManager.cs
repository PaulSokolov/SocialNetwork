using DataLayer.Entities;
using System;

namespace DataLayer.Interfaces
{
    public interface IClientManager : IDisposable
    {
        UserProfile Create(UserProfile item);
        UserProfile Delete(string id);
    }
}


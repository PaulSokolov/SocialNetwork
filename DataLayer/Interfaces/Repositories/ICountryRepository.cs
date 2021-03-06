﻿using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface ICountryRepository : IRepository<Country, long>
    {
    }
}

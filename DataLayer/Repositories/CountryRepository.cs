﻿using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;
using System;
using System.Linq;

namespace DataLayer.Repository
{
    public class CountryRepository : LocalizationRepository<Country>, ICountryRepository
    {
        public CountryRepository(LocalizationContext context) : base(context)
        {
        }

        public Country GetCountry(long id)
        {
            try
            {
                return Context.Countries.Find(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"GetCity() failed: {ex}");
            }
        }

        public IQueryable<Country> GetAll()
        {
            try
            {
                return Context.Countries;
            }
            catch (Exception ex)
            {
                throw new Exception($"GetAll() failed: {ex}");
            }
        }
    }
}

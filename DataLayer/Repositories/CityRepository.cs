﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

namespace DataLayer.Repository
{
    public class CityRepository : LocalizationRepository<City, long>, ICityRepository
    {
        public CityRepository(LocalizationContext context) : base(context)
        {
        }

        //public IQueryable<City> GetAll()
        //{
        //    try
        //    {
        //        return Context.Cities;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"GetAll() failed: {ex}");
        //    }
        //}

        //public IQueryable<City> GetAllCitiesByCountryId(long countryId)
        //{
        //    try
        //    {
        //        return Context.Cities.Where(c => c.CountryId == countryId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"GetAllCitiesByCountryId() failed: {ex}");
        //    }
        //}

        //public City GetCity(long id)
        //{
        //    try
        //    {
        //        return Context.Cities.Find(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"GetCity() failed: {ex}");
        //    }
        //}

        //public async Task<City> GetCityAsync(long id)
        //{
        //    try
        //    {
        //        return await Context.Cities.FindAsync(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"GetCityAsync() failed: {ex}");
        //    }
        //}
    }
}

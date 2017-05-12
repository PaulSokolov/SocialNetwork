using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

namespace DataLayer.Repository
{
    public class CountryRepository : LocalizationRepository<Country, long>, ICountryRepository
    {
        public CountryRepository(LocalizationContext context) : base(context)
        {
        }

        //public Country GetCountry(long id)
        //{
        //    try
        //    {
        //        return Context.Countries.Find(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"GetCity() failed: {ex}");
        //    }
        //}

        //public async Task<Country> GetCountryAsync(long id)
        //{
        //    try
        //    {
        //        return await Context.Countries.FindAsync(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"GetCountryAsync() failed: {ex}");
        //    }
        //}

        //public IQueryable<Country> GetAll()
        //{
        //    try
        //    {
        //        return Context.Countries;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"GetAll() failed: {ex}");
        //    }
        //}

        
    }
}

using System;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface ILocalization : ITransaction, IDisposable
    {
        ICountryRepository GetCountryRepository();
        ICityRepository GetCityRepository();
        ILanguageRepository GetLanguageRepository();
        bool LazyLoad { get; set; }

        Task<ICountryRepository> GetCountryRepositoryAsync();
        Task<ICityRepository> GetCityRepositoryAsync();
        Task<ILanguageRepository> GetLanguageRepositoryAsync();
    }
}

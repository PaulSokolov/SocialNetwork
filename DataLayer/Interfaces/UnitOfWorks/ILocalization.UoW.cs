using System;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface ILocalization : ITransaction, IDisposable
    {
        ICountryRepository GetCountryRepository();
        Task<ICountryRepository> GetCountryRepositoryAsync();
        ICityRepository GetCityRepository();
        Task<ICityRepository> GetCityRepositoryAsync();
        ILanguageRepository GetLanguageRepository();
        Task<ILanguageRepository> GetLanguageRepositoryAsync();
        bool LazyLoad { get; set; }

    }
}

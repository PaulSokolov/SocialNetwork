using System;

namespace DataLayer.Interfaces
{
    public interface ILocalization : ITransaction, IDisposable
    {
        ICountryRepository GetCountryRepository();
        ICityRepository GetCityRepository();
        ILanguageRepository GetLanguageRepository();
        bool LazyLoad { get; set; }
    }
}

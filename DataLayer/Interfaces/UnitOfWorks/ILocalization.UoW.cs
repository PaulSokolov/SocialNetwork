using System;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface ILocalization : ITransaction, IDisposable
    {
        ICountryRepository Countries { get; }
        ICityRepository Cities { get; }
        ILanguageRepository Languages { get; }
        bool LazyLoad { get; set; }

    }
}

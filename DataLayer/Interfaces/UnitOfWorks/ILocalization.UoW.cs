using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

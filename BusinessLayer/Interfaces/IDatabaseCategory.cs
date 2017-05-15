using BusinessLayer.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IDatabaseCategory
    {
        void AddCity(long id, long coutryId, string name);
        void AddCountry(long id, IEnumerable<long> languageIds, string name);
        void AddLanguage(long id, int code, string name);
        List<CityDTO> GetCities();
        List<CityDTO> GetCities(long countryId);
        Task<List<CityDTO>> GetCitiesAsync(long countryId);
        CityDTO GetCityById(long id);
        List<CountryDTO> GetAllCountries();
        Task<List<CountryDTO>> GetAllCountriesAsync();
        CountryDTO GetCountryById(long id);
        List<LanguageDTO> GetLanguages();
        Task<List<LanguageDTO>> GetLanguagesAsync();
        LanguageDTO GetLanguage(long id);
        Task<LanguageDTO> GetLanguageAsync(long id);
    }
}

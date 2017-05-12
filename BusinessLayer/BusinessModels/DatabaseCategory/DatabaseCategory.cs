using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLayer.BusinessModels.Exeptions;
using BusinessLayer.DTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using DataLayer.UnitOfWorks;
using System.Threading.Tasks;

namespace BusinessLayer.BusinessModels
{
    public partial class SocialNetworkFunctionalityUser
    {
        public class DatabaseCategory
        {

            private readonly SocialNetworkFunctionalityUser _socialNetworkFunctionality;
            private readonly ILocalization _localization;

            public DatabaseCategory(SocialNetworkFunctionalityUser socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
                _localization = new Localization(_socialNetworkFunctionality._connection);
            }
            
            public void AddCity(long id, long coutryId, string name)
            {
                var country = _localization.Countries.Get(coutryId);
                if (country == null)
                    throw new DatabaseCategoryItemNotExistException("There is no such country in database category");
                var city = _localization.Cities.Get(id);
                if (city != null)
                    throw new DatabaseCategoryItemAlreadyExistsException("City with this id already exists");
                _localization.Cities.Add(new City { AddedDate = DateTime.Now, Id = id, CountryId = coutryId, Name = name });
                _localization.Commit();

            }

            public void AddCountry(long id, IEnumerable<long> languageIds, string name)
            {
                var country = _localization.Countries.Get(id);
                if (country != null)
                    throw new DatabaseCategoryItemAlreadyExistsException($"Country with Id : {id}. Already exists.");
                var languages = new List<Language>();

                foreach (var languageId in languageIds)
                {
                    var language = _localization.Languages.Get(id);
                    if (language == null)
                        throw new DatabaseCategoryItemNotExistException($"There is no language with Id : {languageId}");
                    languages.Add(_localization.Languages.Get(id));
                }
                _localization.Countries.Add(new Country { AddedDate = DateTime.Now, Id = id, Name = name, Languages = languages });
                _localization.Commit();
            }

            public void AddLanguage(long id, int code, string name)
            {
                var language = _localization.Languages.Get(id);
                if (language != null)
                    throw new DatabaseCategoryItemAlreadyExistsException("Language with Id : {}. Already exists.");
                _localization.Languages.Add(new Language { AddedDate = DateTime.Now, Id = id, Code = code, Name = name });
                _localization.Commit();
            }

            public List<CityDTO> GetCities()
            {
                var cities = _localization.Cities.GetAll().ToList();
                return _socialNetworkFunctionality.Mapper.Map<List<City>, List<CityDTO>>(cities);
            }

            public List<CityDTO> GetCities(long countryId)
            {
                var cities = _localization.Cities.GetAll().Where(c=>c.CountryId == countryId).ToList();
                return _socialNetworkFunctionality.Mapper.Map<List<City>, List<CityDTO>>(cities);
            }

            public async Task<List<CityDTO>> GetCitiesAsync(long countryId)
            {
                var cities = await _localization.Cities.GetAll().Where(c => c.CountryId == countryId).ToListAsync();
                return _socialNetworkFunctionality.Mapper.Map<List<City>, List<CityDTO>>(cities);
            }

            public CityDTO GetCityById(long id)
            {
                City city = _localization.Cities.Get(id);
                return _socialNetworkFunctionality.Mapper.Map<City, CityDTO>(city);
            }

            public List<CountryDTO> GetAllCountries()
            {
                var countries = _localization.Countries.GetAll().ToList();
                return _socialNetworkFunctionality.Mapper.Map<List<Country>, List<CountryDTO>>(countries);
            }

            public async Task<List<CountryDTO>> GetAllCountriesAsync()
            {
                var countries = await _localization.Countries.GetAll().ToListAsync();
                return _socialNetworkFunctionality.Mapper.Map<List<Country>, List<CountryDTO>>(countries);
            }

            public CountryDTO GetCountryById(long id)
            {
                Country country = _localization.Countries.Get(id);
                return _socialNetworkFunctionality.Mapper.Map<Country, CountryDTO>(country);
            }

            public List<LanguageDTO> GetLanguages()
            {
                var languages = _localization.Languages.GetAll().ToList();
                return _socialNetworkFunctionality.Mapper.Map<List<Language>, List<LanguageDTO>>(languages);
            }

            public async Task<List<LanguageDTO>> GetLanguagesAsync()
            {
                var languages = await _localization.Languages.GetAll().ToListAsync();
                return _socialNetworkFunctionality.Mapper.Map<List<Language>, List<LanguageDTO>>(languages);
            }

            public LanguageDTO GetLanguage(long id)
            {
                Language language = _localization.Languages.Get(id);
                return _socialNetworkFunctionality.Mapper.Map<Language, LanguageDTO>(language);
            }

            public async Task<LanguageDTO> GetLanguageAsync(long id)
            {
                Language language = await _localization.Languages.GetAsync(id);
                return _socialNetworkFunctionality.Mapper.Map<Language, LanguageDTO>(language);
            }

        }
    }
}
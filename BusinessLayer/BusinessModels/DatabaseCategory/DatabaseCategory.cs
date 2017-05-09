using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLayer.BusinessModels.Exeptions;
using BusinessLayer.DTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using DataLayer.UnitOfWorks;

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
                var country = _localization.GetCountryRepository().GetCountry(coutryId);
                if (country == null)
                    throw new DatabaseCategoryItemNotExistException("There is no such country in database category");
                var city = _localization.GetCityRepository().GetCity(id);
                if (city != null)
                    throw new DatabaseCategoryItemAlreadyExistsException("City with this id already exists");
                _localization.GetCityRepository().Add(new City { AddedDate = DateTime.Now, Id = id, CountryId = coutryId, Name = name });
                _localization.Commit();

            }

            public void AddCountry(long id, IEnumerable<long> languageIds, string name)
            {
                var country = _localization.GetCountryRepository().GetCountry(id);
                if (country != null)
                    throw new DatabaseCategoryItemAlreadyExistsException($"Country with Id : {id}. Already exists.");
                var languages = new List<Language>();

                foreach (var languageId in languageIds)
                {
                    var language = _localization.GetLanguageRepository().GetLanguage(id);
                    if (language == null)
                        throw new DatabaseCategoryItemNotExistException($"There is no language with Id : {languageId}");
                    languages.Add(_localization.GetLanguageRepository().GetLanguage(id));
                }
                _localization.GetCountryRepository().Add(new Country { AddedDate = DateTime.Now, Id = id, Name = name, Languages = languages });
                _localization.Commit();
            }

            public void AddLanguage(long id, int code, string name)
            {
                var language = _localization.GetLanguageRepository().GetLanguage(id);
                if (language != null)
                    throw new DatabaseCategoryItemAlreadyExistsException("Language with Id : {}. Already exists.");
                _localization.GetLanguageRepository().Add(new Language { AddedDate = DateTime.Now, Id = id, Code = code, Name = name });
                _localization.Commit();
            }

            public List<CityDTO> GetCities()
            {
                var cities = _localization.GetCityRepository().GetAll().ToList();
                return _socialNetworkFunctionality.Mapper.Map<List<City>, List<CityDTO>>(cities);
            }

            public List<CityDTO> GetCities(long countryId)
            {
                var cities = _localization.GetCityRepository().GetAll().Where(c=>c.CountryId == countryId).ToList();
                return _socialNetworkFunctionality.Mapper.Map<List<City>, List<CityDTO>>(cities);
            }

            public CityDTO GetCityById(long id)
            {
                City city = _localization.GetCityRepository().GetCity(id);
                return _socialNetworkFunctionality.Mapper.Map<City, CityDTO>(city);
            }

            public List<CountryDTO> GetAllCountries()
            {
                var countries = _localization.GetCountryRepository().GetAll().ToList();
                return _socialNetworkFunctionality.Mapper.Map<List<Country>, List<CountryDTO>>(countries);
            }

            public CountryDTO GetCountryById(long id)
            {
                Country country = _localization.GetCountryRepository().GetCountry(id);
                return _socialNetworkFunctionality.Mapper.Map<Country, CountryDTO>(country);
            }

            public List<LanguageDTO> GetLanguages()
            {
                var languages = _localization.GetLanguageRepository().GetAll().ToList();
                return _socialNetworkFunctionality.Mapper.Map<List<Language>, List<LanguageDTO>>(languages);
            }

            public LanguageDTO GetLanguageById(long id)
            {
                Language language = _localization.GetLanguageRepository().GetLanguage(id);
                return _socialNetworkFunctionality.Mapper.Map<Language, LanguageDTO>(language);
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using BusinessLayer.BusinessModels.Exeptions;
using BusinessLayer.DTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using DataLayer.UnitOfWorks;
using System.Threading.Tasks;
using AutoMapper;

namespace BusinessLayer.BusinessModels
{
    public partial class SocialNetworkFunctionalityUser
    {
        public class DatabaseCategory
        {
            private SemaphoreSlim _semaphore;
            private SemaphoreSlim Semophore => _semaphore ?? (_semaphore = new SemaphoreSlim(Threads, MaxThreads));
            private readonly SocialNetworkFunctionalityUser _socialNetworkFunctionality;
            //private readonly ILocalization _localization;
            private ILocalization Localization => _socialNetworkFunctionality._localizationConnection ??
                                                  (_socialNetworkFunctionality._localizationConnection =
                                                      new Localization(Connection));

            private IMapper Mapper => _socialNetworkFunctionality._mapper;

            public DatabaseCategory(SocialNetworkFunctionalityUser socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
            }
            
            public void AddCity(long id, long coutryId, string name)
            {
                var country = Localization.Countries.Get(coutryId);
                if (country == null)
                    throw new DatabaseCategoryItemNotExistException("There is no such country in database category");
                var city = Localization.Cities.Get(id);
                if (city != null)
                    throw new DatabaseCategoryItemAlreadyExistsException("City with this id already exists");
                Localization.Cities.Add(new City { AddedDate = DateTime.Now, Id = id, CountryId = coutryId, Name = name });
                Localization.Commit();

            }

            public void AddCountry(long id, IEnumerable<long> languageIds, string name)
            {
                var country = Localization.Countries.Get(id);
                if (country != null)
                    throw new DatabaseCategoryItemAlreadyExistsException($"Country with Id : {id}. Already exists.");
                var languages = new List<Language>();

                foreach (var languageId in languageIds)
                {
                    var language = Localization.Languages.Get(id);
                    if (language == null)
                        throw new DatabaseCategoryItemNotExistException($"There is no language with Id : {languageId}");
                    languages.Add(Localization.Languages.Get(id));
                }
                Localization.Countries.Add(new Country { AddedDate = DateTime.Now, Id = id, Name = name, Languages = languages });
                Localization.Commit();
            }

            public void AddLanguage(long id, int code, string name)
            {
                var language = Localization.Languages.Get(id);
                if (language != null)
                    throw new DatabaseCategoryItemAlreadyExistsException("Language with Id : {}. Already exists.");
                Localization.Languages.Add(new Language { AddedDate = DateTime.Now, Id = id, Code = code, Name = name });
                Localization.Commit();
            }

            public List<CityDTO> GetCities()
            {
                var cities = Localization.Cities.GetAll().ToList();
                return Mapper.Map<List<City>, List<CityDTO>>(cities);
            }

            public List<CityDTO> GetCities(long countryId)
            {
                var cities = Localization.Cities.GetAll().Where(c=>c.CountryId == countryId).ToList();
                return Mapper.Map<List<City>, List<CityDTO>>(cities);
            }

            public async Task<List<CityDTO>> GetCitiesAsync(long countryId)
            {
                if (Semophore.CurrentCount == Threads)
                {
                    await Semophore.WaitAsync();
                    var cities = await Localization.Cities.GetAll().Where(c => c.CountryId == countryId).ToListAsync();
                    Semophore.Release();
                    return Mapper.Map<List<City>, List<CityDTO>>(cities);
                }
                using (var context = new Localization(Connection))
                {
                    var cities = await context.Cities.GetAll().Where(c => c.CountryId == countryId)
                        .ToListAsync();
                    return Mapper.Map<List<City>, List<CityDTO>>(cities);
                }
            }

            public CityDTO GetCityById(long id)
            {
                City city = Localization.Cities.Get(id);
                return Mapper.Map<City, CityDTO>(city);
            }

            public List<CountryDTO> GetAllCountries()
            {
                var countries = Localization.Countries.GetAll().ToList();
                return Mapper.Map<List<Country>, List<CountryDTO>>(countries);
            }

            public async Task<List<CountryDTO>> GetAllCountriesAsync()
            {
                if (Semophore.CurrentCount == Threads)
                {
                    await Semophore.WaitAsync();
                    var countries = await Localization.Countries.GetAll().ToListAsync();
                    Semophore.Release();
                    return Mapper.Map<List<Country>, List<CountryDTO>>(countries);
                }
                using (var context = new Localization(Connection))
                {
                    var countries = await context.Countries.GetAll().ToListAsync();
                    return _socialNetworkFunctionality._mapper.Map<List<Country>, List<CountryDTO>>(countries);
                }
            }

            public CountryDTO GetCountryById(long id)
            {
                Country country = Localization.Countries.Get(id);
                return Mapper.Map<Country, CountryDTO>(country);
            }

            public List<LanguageDTO> GetLanguages()
            {
                var languages = Localization.Languages.GetAll().ToList();
                return Mapper.Map<List<Language>, List<LanguageDTO>>(languages);
            }

            public async Task<List<LanguageDTO>> GetLanguagesAsync()
            {
                var languages = await Localization.Languages.GetAll().ToListAsync();
                return Mapper.Map<List<Language>, List<LanguageDTO>>(languages);
            }

            public LanguageDTO GetLanguage(long id)
            {
                Language language = Localization.Languages.Get(id);
                return Mapper.Map<Language, LanguageDTO>(language);
            }

            public async Task<LanguageDTO> GetLanguageAsync(long id)
            {
                Language language = await Localization.Languages.GetAsync(id);
                return Mapper.Map<Language, LanguageDTO>(language);
            }

        }
    }
}
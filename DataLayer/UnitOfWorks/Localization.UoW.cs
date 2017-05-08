using DataLayer.Interfaces;
using DataLayer.EF;
using DataLayer.Repository;
using DataLayer.Entities;
using System.Collections.Generic;

namespace DataLayer.UnitOfWorks
{
    public class Localization : ILocalization
    {
        private ICityRepository _cityRepository;
        private ICountryRepository _countryRepository;
        private ILanguageRepository _languageRepository;
        protected readonly LocalizationContext _context;

        public bool LazyLoad { get => _context.Configuration.LazyLoadingEnabled; set => _context.Configuration.LazyLoadingEnabled = value; }

        public Localization(string connection)
        {
            _context = new LocalizationContext(connection);
        }
        public void Dispose()
        {
            if (_cityRepository != null)
                _cityRepository.Dispose();
            if (_countryRepository != null)
                _countryRepository.Dispose();
            if (_languageRepository != null)
                _languageRepository.Dispose();
        }
        #region ITransaction
        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Remove<T>(T entity) where T : Entity
        {
            _context.Set<T>().Remove(entity);
        }

        public void RemoveRange<T>(IEnumerable<T> entities) where T : Entity
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public T Add<T>(T entity) where T : Entity
        {
            return _context.Set<T>().Add(entity);
        }

        public IEnumerable<T> AddRange<T>(IEnumerable<T> entities) where T : Entity
        {
            return _context.Set<T>().AddRange(entities);
        }
        #endregion
        #region ILocalization
        public ICityRepository GetCityRepository()
        {
            return _cityRepository ?? (_cityRepository = new CityRepository(_context));
        }

        public ICountryRepository GetCountryRepository()
        {
            return _countryRepository ?? (_countryRepository = new CountryRepository(_context));
        }

        public ILanguageRepository GetLanguageRepository()
        {
            return _languageRepository ?? (_languageRepository = new LanguageRepository(_context));
        }
        #endregion
    }
}

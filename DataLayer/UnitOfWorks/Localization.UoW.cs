using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;
using DataLayer.Repository;

namespace DataLayer.UnitOfWorks
{
    public class Localization : ILocalization
    {
        private ICityRepository _cityRepository;
        private ICountryRepository _countryRepository;
        private ILanguageRepository _languageRepository;
        private readonly LocalizationContext _context;

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

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public T Remove<T>(T entity) where T : Entity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return _context.Set<T>().Remove(entity);
        }

        public async Task<T> RemoveAsync<T>(T entity) where T : Entity
        {
            var task = new Task<T>(()=>Remove(entity));
            task.Start();
            return await task;
        }

        public IEnumerable<T> RemoveRange<T>(IEnumerable<T> entities) where T : Entity
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            return _context.Set<T>().RemoveRange(entities);
        }

        public async Task<IEnumerable<T>> RemoveRangeAsync<T>(IEnumerable<T> entities) where T : Entity
        {
            var task = new Task<IEnumerable<T>>(() => RemoveRange(entities));
            task.Start();
            return await task;
        }

        public T Add<T>(T entity) where T : Entity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return _context.Set<T>().Add(entity);
        }

        public async Task<T> AddAsync<T>(T entity) where T : Entity
        {
            var task = new Task<T>(()=>Add(entity));
            task.Start();

            return await task;
        }

        public IEnumerable<T> AddRange<T>(IEnumerable<T> entities) where T : Entity
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            return _context.Set<T>().AddRange(entities);
        }

        public async Task<IEnumerable<T>> AddRangeAsync<T>(IEnumerable<T> entities) where T : Entity
        {
            var task = new Task<IEnumerable<T>>(() => AddRange(entities));
            task.Start();

            return await task;
        }
        #endregion
        #region ILocalization
        public ICityRepository GetCityRepository()
        {
            return _cityRepository ?? (_cityRepository = new CityRepository(_context));
        }

        public async Task<ICityRepository> GetCityRepositoryAsync()
        {
            var task = new Task<ICityRepository>(GetCityRepository);
            task.Start();
            return await task;
        }

        public ICountryRepository GetCountryRepository()
        {
            return _countryRepository ?? (_countryRepository = new CountryRepository(_context));
        }

        public async Task<ICountryRepository> GetCountryRepositoryAsync()
        {
            var task = new Task<ICountryRepository>(GetCountryRepository);
            task.Start();
            return await task;
        }

        public ILanguageRepository GetLanguageRepository()
        {
            return _languageRepository ?? (_languageRepository = new LanguageRepository(_context));
        }

        public async Task<ILanguageRepository> GetLanguageRepositoryAsync()
        {
            var task = new Task<ILanguageRepository>(GetLanguageRepository);
            task.Start();
            return await task;
        }
        #endregion
    }
}

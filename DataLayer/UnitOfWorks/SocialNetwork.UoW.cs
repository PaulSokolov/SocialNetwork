using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;
using DataLayer.Repository;

namespace DataLayer.UnitOfWorks
{
    public class SocialNetwork : ISocialNetwork
    {
        
        #region Variables

        private IFriendRepository _friendRepository;
        private IUserMessageRepository _userMessageRepository;
        private IUserProfileRepository _userProfileRepository;
        private readonly UserProfileContext _context;
        #endregion
        public SocialNetwork(string connection)
        {
            _context = new UserProfileContext(connection);
        }
        public virtual void Dispose()
        {
            if (_friendRepository != null)
                _friendRepository.Dispose();
            if (_userMessageRepository != null)
                _userMessageRepository.Dispose();
            if (_userProfileRepository != null)
                _userProfileRepository.Dispose();

        }
        #region ISocialNetwork
        public bool LazyLoad
        {
            get => _context.Configuration.LazyLoadingEnabled;
            set => _context.Configuration.LazyLoadingEnabled = value;
        }

        public IFriendRepository Friends => _friendRepository ?? (_friendRepository = new FriendRepository(_context));

        public IUserMessageRepository Messages => _userMessageRepository ?? (_userMessageRepository = new UserMessageRepository(_context));

        public IUserProfileRepository UserProfiles => _userProfileRepository ?? (_userProfileRepository = new UserProfileRepository(_context));
        #endregion
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
            var task = new Task<T>(() => Remove(entity));
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
            var task = new Task<T>(() => Add(entity));
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
    }
}

using System.Collections.Generic;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;
using DataLayer.Repository;

namespace DataLayer.UnitOfWorks
{
    public class SocialNetwork : ISocialNetwork
    {
        public bool LazyLoad
        {
            get => _context.Configuration.LazyLoadingEnabled;
            set => _context.Configuration.LazyLoadingEnabled = value;
        }
        #region Variables

        private IFriendRepository _friendRepository;
        private IUserMessageRepository _userMessageRepository;
        private IUserProfileRepository _userProfileRepository;
        protected readonly UserProfileContext _context;
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
        public IUserMessageRepository GetUserMessageRepository()
        {
            return _userMessageRepository ?? (_userMessageRepository = new UserMessageRepository(_context));
        }

        public IUserProfileRepository GetUserProfileRepository()
        {
            return _userProfileRepository ?? (_userProfileRepository = new UserProfileRepository(_context));
        }

        public IFriendRepository GetFriendRepository()
        {
            return _friendRepository ?? (_friendRepository = new FriendRepository(_context));
        }
        #endregion
        #region ITransaction
        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Remove<T>(T entity) where T : Entity
        {
            _context.Set<T>().Attach(entity);
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
    }
}

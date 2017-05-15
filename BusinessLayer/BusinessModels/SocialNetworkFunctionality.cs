using System;
using System.IO;
using System.Threading;
using AutoMapper;
using BusinessLayer.Interfaces;
using DataLayer.Interfaces;
using DataLayer.UnitOfWorks;

namespace BusinessLayer.BusinessModels
{
    public partial class SocialNetworkManager:ISocialNetworkManager
    {
        private const string Connection = "name=SocialNetwork";

        #region Private fields
        private readonly IMapper _mapper;
        private ISocialNetwork _socialNetwork;
        private ILocalization _localizationConnection;
        private IFriendsCategory _friends;
        private IUsersCategory _users;
        private IMessagesCategory _messages;
        private IDatabaseCategory _database;
        private readonly Func<DateTime> _now;
        private SemaphoreSlim _semaphore;
        private const int Threads = 1;
        private const int MaxThreads = 1;
        #endregion

        public string Id { get; set; }
        public IFriendsCategory Friends
        {
            get
            {
                if (_friends != null) return _friends;

                _friends = new FriendsCategory(this);
                return _friends;
            }
        }
        public IUsersCategory Users
        {
            get
            {
                if (_users != null) return _users;
                _users = new UsersCategory(this);
                return _users;
            }
        }
        public IMessagesCategory Messages
        {
            get
            {
                if (_messages != null) return _messages;

                _messages = new MessagesCategory(this);
                return _messages;
            }
        }
        public IDatabaseCategory Database
        {
            get
            {
                if (_database != null) return _database;
                _database = new DatabaseCategory(this);
                return _database;
            }
        }

        public SocialNetworkManager()
        {
            _mapper = CustomMapper.Configurate();
            _now = () => DateTime.Now;
            _semaphore = new SemaphoreSlim(Threads, MaxThreads);
        }

        public SocialNetworkManager(string userId)
        {
            Id = userId;            
            _mapper = CustomMapper.Configurate();
            _now = () => DateTime.Now;
            _semaphore = new SemaphoreSlim(Threads, MaxThreads);
        }

        public SocialNetworkManager(string userId, ISocialNetwork socialNetworkUoW, ILocalization localizationUoW, Func<DateTime> now)
        {
            Id = userId;
            _socialNetwork = socialNetworkUoW;
            _localizationConnection = localizationUoW;
            _mapper = CustomMapper.Configurate();
            _semaphore = new SemaphoreSlim(Threads, MaxThreads);
            _now = now;
        }
    }
    
   
}
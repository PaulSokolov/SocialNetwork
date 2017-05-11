using System;
using System.IO;
using AutoMapper;
using DataLayer.Interfaces;

namespace BusinessLayer.BusinessModels
{
    public partial class SocialNetworkFunctionalityUser
    {
        private readonly string _connection = "name=SocialNetwork";

        #region Private fields
        private IMapper Mapper;
        private ISocialNetwork _socialNetworkConnection;
        private ILocalization _localizationConnection;
        private FriendsCategory _friends;
        private UsersCategory _users;
        private MessagesCategory _messages;
        private DatabaseCategory _database; 
        #endregion

        public string Id { get; }
        public FriendsCategory Friends
        {
            get
            {
                if (_friends != null) return _friends;

                _friends = new FriendsCategory(this);
                return _friends;
            }
            private set => _friends = value;
        }
        public UsersCategory Users
        {
            get
            {
                if (_users != null) return _users;
                _users = new UsersCategory(this);
                return _users;
            }
            private set => _users = value;
        }
        public MessagesCategory Messages
        {
            get
            {
                if (_messages != null) return _messages;

                _messages = new MessagesCategory(this);
                return _messages;
            }
            private set => _messages = value;
        }
        public DatabaseCategory Database
        {
            get
            {
                if (_database != null) return _database;
                _database = new DatabaseCategory(this);
                return _database;
            }
            private set => _database = value;
        }

        private readonly Func<DateTime> _now;

        public SocialNetworkFunctionalityUser(string userId)
        {
            Id = userId;            
            Mapper = CustomMapper.Configurate();
            _now = () => DateTime.Now;
        }

        public SocialNetworkFunctionalityUser(string userId, ISocialNetwork socialNetworkUoW, ILocalization localizationUoW, Func<DateTime> now)
        {
            Id = userId;
            _socialNetworkConnection = socialNetworkUoW;
            _localizationConnection = localizationUoW;
            Mapper = CustomMapper.Configurate();
            Friends = new FriendsCategory(this);
            Users = new UsersCategory(this);
            Messages = new MessagesCategory(this);
            _now = now;
        }
    }
    
   
}
using DataLayer.Interfaces;
using NUnit.Framework;
using Moq;
using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using System;
using DataLayer.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer;

namespace BLLTests
{

    [TestFixture]
    public class SocialNetworkFriendsCategoryTests
    {
        private IMapper Mapper { get; set; }
        Func<DateTime> NowAction => () => new DateTime(2011, 11, 11);

        private DateTime Now => NowAction();
        //SocialNetworkFunctionalityUser socialNetwork;
        List<UserProfile> userProfiles;
        List<Friend> friends;
        List<City> cities;
        List<Country> countries;
        List<Language> languages;
        List<UserMessage> messages;

        static readonly DateTime time = new DateTime(2011, 11, 11);
        static readonly Entity entity;
        static readonly string userId = "1";
        
        [OneTimeSetUp]
        public void Init()
        {
            Mapper = CustomMapper.Configurate();
            friends = new List<Friend> {
                new Friend{
                    AddedDate = time,
                    RequestDate = time,
                    RequestUserId = "1",
                    FriendId = "2",
                    Confirmed = false,
                    UserId = "1",
                },
                new Friend{
                    AddedDate = time,
                    RequestDate = time,
                    RequestUserId = "2",
                    FriendId = "4",
                    Confirmed = false,
                    UserId = "2",
                },
                new Friend{
                    AddedDate = time,
                    RequestDate = time,
                    RequestUserId = "2",
                    FriendId = "4",
                    Confirmed = false,
                    UserId = "2",
                }
            };
            InitTestData();

        }
        [Test]
        public async Task AddFriend_Add_AsExpected()
        {
            string userToAddId = "userToAddId";
            var moqUserProfileRepository = new Mock<IUserProfileRepository>();
            moqUserProfileRepository.Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserProfile
                {
                    Id = "userToAddId",
                    BirthDate = new DateTime(1996, 7, 11),
                    Name = "Paul",
                    LastName = "Sokolov",
                    Sex = Sex.Male,
                    CityId = 1,
                    Email = "www.pashasokolov@mail.ru"
                });
            var moqFriendRepository = new Mock<IFriendRepository>();
            moqFriendRepository.Setup(m => m.AddAsync(It.IsAny<Friend>())).ReturnsAsync(new Friend
            {
                    AddedDate = Now,
                    RequestDate = Now,
                    RequestUserId = userId,
                    FriendId = userToAddId,
                    Confirmed = false,
                    UserId = userId
            });
            var moqSocialNetwork = new Mock<ISocialNetwork>();
            moqSocialNetwork.Setup(m => m.Friends).Returns(moqFriendRepository.Object);
            moqSocialNetwork.Setup(m => m.UserProfiles).Returns(moqUserProfileRepository.Object);
            var socialNetwork = new SocialNetworkManager(userId, moqSocialNetwork.Object, null, () => new DateTime(2011, 11, 11));
            
            Friend friend = new Friend
            {
                AddedDate = Now,
                RequestDate = Now,
                RequestUserId = userId,
                FriendId = userToAddId,
                Confirmed = false,
                UserId = userId,
            };
            FriendDTO result = await socialNetwork.Friends.AddAsync(userToAddId);
            var expected = Mapper.Map<Friend, FriendDTO>(friend);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ConfirmRequest_UserNotFountException()
        {
            var data = new List<UserProfile>().AsQueryable();
            var mockSetForUserProfile = TestAsyncQueryManger.MoqSetCreator(data);

            var moqUserProfileRepository = new Mock<IUserProfileRepository>();
            moqUserProfileRepository.Setup(m => m.GetAll()).Returns(mockSetForUserProfile.Object);
            var moqSocialNetwork = new Mock<ISocialNetwork>();
            moqSocialNetwork.Setup(m => m.UserProfiles).Returns(moqUserProfileRepository.Object);

            var socialNetwork = new SocialNetworkManager("", moqSocialNetwork.Object, null, NowAction);
            Assert.Throws(typeof(UserNotFoundException),  () =>  socialNetwork.Friends.ConfirmAsync(It.IsAny<long>()).GetAwaiter().GetResult());
        }
        [Test]
        public async Task GetCurrentUserFriends_GetFriendsAsync_GetFriendsAsExpected()
        {
            var expected = new UserProfile
            {
                Id = "2",
                PublicId = 2,
                Name = "Ruslan"
            };
            var userProfiles = new List<UserProfile>
            {
                new UserProfile
                {
                    Id = "1",
                    PublicId = 1,
                    Name = "Paul"
                },
                new UserProfile
                {
                    Id = "2",
                    PublicId = 2,
                    Name = "Ruslan"
                }
            };
            var friends = new List<Friend>
            {
                new Friend{
                    UserId = "1",
                    RequestUserId = "1",
                    FriendId = "2",
                    Confirmed = true,
                    Deleted = false
                },
                new Friend
                {
                    UserId = "2",
                    RequestUserId = "1",
                    FriendId = "1",
                    Confirmed = true,
                    Deleted = false
                }
            };

            var usersQuery = userProfiles.AsQueryable();
            var friendsQuery = friends.AsQueryable();

            var mockSetForUserProfiles = TestAsyncQueryManger.MoqSetCreator(usersQuery);
            var mockSetForFriends = TestAsyncQueryManger.MoqSetCreator(friendsQuery);

            

            var moqFriendRepository = new Mock<IFriendRepository>();
            moqFriendRepository.Setup(m => m.GetAll()).Returns(mockSetForFriends.Object);
            var moqUserProfileRepository = new Mock<IUserProfileRepository>();
            moqUserProfileRepository.Setup(m => m.GetAll()).Returns(mockSetForUserProfiles.Object);

            var moqSocialNetwork = new Mock<ISocialNetwork>();
            moqSocialNetwork.Setup(m => m.Friends).Returns(moqFriendRepository.Object);
            moqSocialNetwork.Setup(m => m.UserProfiles).Returns(moqUserProfileRepository.Object);

            var socialNetwork = new SocialNetworkManager("1", moqSocialNetwork.Object, null, NowAction);
            var result = await socialNetwork.Friends.GetFriendsAsync();
            Assert.AreEqual(expected.Id, result.First().Id);
            Assert.AreEqual(expected.Name, result.First().Name);
            Assert.AreEqual(expected.PublicId, result.First().PublicId);
            Assert.AreEqual(Mapper.Map<UserProfileDTO>(expected), result.First());
        }

        [Test]
        public async Task GetCurrentUserFollowers_GetFollowersAsync_GetFriendsAsExpected()
        {
            var expected = new UserProfile
            {
                Id = "2",
                PublicId = 2,
                Name = "Ruslan"
            };
            var userProfiles = new List<UserProfile>
            {
                new UserProfile
                {
                    Id = "1",
                    PublicId = 1,
                    Name = "Paul"
                },
                new UserProfile
                {
                    Id = "2",
                    PublicId = 2,
                    Name = "Ruslan"
                }
            };
            var friends = new List<Friend>
            {
                new Friend{
                    UserId = "1",
                    RequestUserId = "2",
                    FriendId = "2",
                    Confirmed = false,
                    Deleted = false
                },
                new Friend
                {
                    UserId = "2",
                    RequestUserId = "2",
                    FriendId = "1",
                    Confirmed = false,
                    Deleted = false
                }
            };

            var usersQuery = userProfiles.AsQueryable();
            var friendsQuery = friends.AsQueryable();

            var mockSetForUserProfiles = TestAsyncQueryManger.MoqSetCreator(usersQuery);
            var mockSetForFriends = TestAsyncQueryManger.MoqSetCreator(friendsQuery);



            var moqFriendRepository = new Mock<IFriendRepository>();
            moqFriendRepository.Setup(m => m.GetAll()).Returns(mockSetForFriends.Object);
            var moqUserProfileRepository = new Mock<IUserProfileRepository>();
            moqUserProfileRepository.Setup(m => m.GetAll()).Returns(mockSetForUserProfiles.Object);

            var moqSocialNetwork = new Mock<ISocialNetwork>();
            moqSocialNetwork.Setup(m => m.Friends).Returns(moqFriendRepository.Object);
            moqSocialNetwork.Setup(m => m.UserProfiles).Returns(moqUserProfileRepository.Object);

            var socialNetwork = new SocialNetworkManager("1", moqSocialNetwork.Object, null, NowAction);
            var result = await socialNetwork.Friends.GetFollowersAsync();
            Assert.AreEqual(expected.Id, result.First().Id);
            Assert.AreEqual(expected.Name, result.First().Name);
            Assert.AreEqual(expected.PublicId, result.First().PublicId);
            Assert.AreEqual(Mapper.Map<UserProfileDTO>(expected), result.First());
        }

        [Test]
        public async Task GetCurrentUserFollowedUsers_GetFollowedAsync_GetFriendsAsExpected()
        {
            var expected = new UserProfile
            {
                Id = "2",
                PublicId = 2,
                Name = "Ruslan"
            };
            var userProfiles = new List<UserProfile>
            {
                new UserProfile
                {
                    Id = "1",
                    PublicId = 1,
                    Name = "Paul"
                },
                new UserProfile
                {
                    Id = "2",
                    PublicId = 2,
                    Name = "Ruslan"
                }
            };
            var friends = new List<Friend>
            {
                new Friend{
                    UserId = "1",
                    RequestUserId = "1",
                    FriendId = "2",
                    Confirmed = false,
                    Deleted = true
                },
                new Friend
                {
                    UserId = "2",
                    RequestUserId = "1",
                    FriendId = "1",
                    Confirmed = false,
                    Deleted = true
                }
            };

            var usersQuery = userProfiles.AsQueryable();
            var friendsQuery = friends.AsQueryable();

            var mockSetForUserProfiles = TestAsyncQueryManger.MoqSetCreator(usersQuery);
            var mockSetForFriends = TestAsyncQueryManger.MoqSetCreator(friendsQuery);



            var moqFriendRepository = new Mock<IFriendRepository>();
            moqFriendRepository.Setup(m => m.GetAll()).Returns(mockSetForFriends.Object);
            var moqUserProfileRepository = new Mock<IUserProfileRepository>();
            moqUserProfileRepository.Setup(m => m.GetAll()).Returns(mockSetForUserProfiles.Object);

            var moqSocialNetwork = new Mock<ISocialNetwork>();
            moqSocialNetwork.Setup(m => m.Friends).Returns(moqFriendRepository.Object);
            moqSocialNetwork.Setup(m => m.UserProfiles).Returns(moqUserProfileRepository.Object);

            var socialNetwork = new SocialNetworkManager("1", moqSocialNetwork.Object, null, NowAction);
            var result = await socialNetwork.Friends.GetFollowedAsync();
            Assert.AreEqual(expected.Id, result.First().Id);
            Assert.AreEqual(expected.Name, result.First().Name);
            Assert.AreEqual(expected.PublicId, result.First().PublicId);
            Assert.AreEqual(Mapper.Map<UserProfileDTO>(expected),result.First());
        }

        
      

        private void InitTestData()
        {
            UserProfile user1 = new UserProfile
            {
                Id = "1",
                BirthDate = new DateTime(1996, 7, 11),
                Name = "Paul",
                LastName = "Sokolov",
                Sex = Sex.Male,
                CityId = 1,
                //City = cities[0],
                //Languages = languages.Where(l => l.Id == 1 && l.Id == 2).ToList(),
                Email = "www.pashasokolov@mail.ru",
                //Messages = messages.Where(m => m.FromUserId == "1").ToList(),
                //Friends = friends.Where(f => f.UserId == "1").ToList()
            };
            UserProfile user2 = new UserProfile
            {
                Id = "2",
                BirthDate = new DateTime(1966, 7, 11),
                Name = "Ruslan",
                LastName = "Kovalenko",
                Sex = Sex.Male,
                CityId = 2,
                //City = cities[1],
                //Languages = languages.Where(l => l.Id == 1 && l.Id == 3).ToList(),
                Email = "www.rus.koval@mail.ru",
                //Messages = messages.Where(m => m.FromUserId == "2").ToList(),
                //Friends = friends.Where(f => f.UserId == "2").ToList()
            };
            UserProfile user3 = new UserProfile
            {
                Id = "3",
                BirthDate = new DateTime(1996, 12, 25),
                Name = "Alina",
                LastName = "Koshevaya",
                Sex = Sex.Female,
                CityId = 3,
                //City = cities[2],
                //Languages = languages.Where(l => l.Id == 1).ToList(),
                Email = "www.al.kshv@mail.ru",
                //Messages = messages.Where(m => m.FromUserId == "3").ToList(),
                //Friends = friends.Where(f => f.UserId == "3").ToList()
            };
            UserProfile user4 = new UserProfile
            {
                Id = "4",
                BirthDate = new DateTime(1996, 7, 11),
                Name = "Vlad",
                LastName = "Dyshlyk",
                Sex = Sex.Male,
                CityId = 4,
                //City = cities[3],
                //Languages = languages.Where(l => l.Id == 3).ToList(),
                Email = "www.vld@mail.ru",
                //Messages = messages.Where(m => m.FromUserId == "4").ToList(),
                //Friends = friends.Where(f => f.UserId == "4").ToList()
            };

            
            languages = new List<Language> {
                new Language{
                    Id = 1,
                    Code = 1,
                    Name = "Ukrainian"
                },
                new Language{
                    Id = 2,
                    Code = 2,
                    Name = "Russian"
                },
                new Language{
                    Id = 3,
                    Code = 3,
                    Name = "English"
                }
            };
            countries = new List<Country> {
                new Country{
                    Id = 1,
                    Languages = languages.Where(l=>l.Id==1 && l.Id==2).ToList(),
                    Name ="Ukraine"
                },
                new Country{
                    Id = 2,
                    Languages = languages.Where(l=>l.Id==2).ToList(),
                    Name ="Russia"
                }
            };
            cities = new List<City> {
                new City{
                    Id=1,
                    CountryId = 1,
                    Country = countries[0],
                    Name = "Vinnitsa"
                },
                new City{
                    Id=2,
                    CountryId = 1,
                    Country = countries[0],
                    Name = "Kyiv"
                },
                new City{
                    Id=3,
                    CountryId = 1,
                    Country = countries[0],
                    Name = "Dnepr"
                },
                new City{
                    Id=4,
                    CountryId = 2,
                    Country = countries[0],
                    Name = "Moscow"
                },
                new City{
                    Id=5,
                    CountryId = 2,
                    Country = countries[0],
                    Name = "Saint`s Petersburg"
                }
            };
            
            messages = new List<UserMessage> {
                new UserMessage{
                    Id = 1,
                    FromUserId = "1",
                    FromUser = user1,
                    ToUserId = "2",
                    ToUser = user2,
                    IsRead =false,
                    Body = "Hello, Rusya"
                },
                new UserMessage{
                    Id = 8,
                    FromUserId = "1",
                    FromUser = user1,
                    ToUserId = "2",
                    ToUser =user2,
                    IsRead =false,
                    Body = "Hello, Rusya"
                },
                new UserMessage{
                    Id = 2,
                    FromUserId = "2",
                    FromUser = user2,
                    ToUserId = "1",
                    ToUser = user1,
                    IsRead =false,
                    Body = "Hello, Pasha"
                },
                new UserMessage{
                    Id = 3,
                    FromUserId = "1",
                    FromUser = user1,
                    ToUserId = "3",
                    ToUser = user3,
                    IsRead =false,
                    Body = "Hello, Alina"
                },
                new UserMessage{
                    Id = 4,
                    FromUserId = "3",
                    FromUser = user3,
                    ToUserId = "1",
                    ToUser = user1,
                    IsRead =false,
                    Body = "Hello, Pasha"
                },
                new UserMessage{
                    Id = 5,
                    FromUserId = "4",
                    FromUser = user4,
                    ToUserId = "2",
                    ToUser = user2,
                    IsRead =false,
                    Body = "Hello, Rusya"
                },
                new UserMessage{
                    Id = 6,
                    FromUserId = "4",
                    FromUser = user4,
                    ToUserId = "2",
                    ToUser = user2,
                    IsRead =false,
                    Body = "Hello, Rusya"
                },
                new UserMessage{
                    Id = 7,
                    FromUserId = "4",
                    FromUser = user4,
                    ToUserId = "2",
                    ToUser = user2,
                    IsRead =false,
                    Body = "Hello, Rusya"
                }
            };

            userProfiles = new List<UserProfile> {
                new UserProfile{
                    Id="1",
                    BirthDate=new DateTime(1996,7,11),
                    Name="Paul",
                    LastName="Sokolov",
                    Sex=Sex.Male,
                    CityId=1,
                    City =cities[0],
                    Languages=languages.Where(l=>l.Id==1 && l.Id == 2).ToList(),
                    Email="www.pashasokolov@mail.ru",
                    SentMessages=messages.Where(m=>m.FromUserId=="1").ToList(),
                    Friends=friends.Where(f=>f.UserId == "1").ToList()
                },
                new UserProfile{
                    Id="2",
                    BirthDate=new DateTime(1966,7,11),
                    Name="Ruslan",
                    LastName="Kovalenko",
                    Sex=Sex.Male,
                    CityId=2,
                    City =cities[1],
                    Languages=languages.Where(l=>l.Id==1 && l.Id == 3).ToList(),
                    Email="www.rus.koval@mail.ru",
                    SentMessages=messages.Where(m=>m.FromUserId=="2").ToList(),
                    Friends=friends.Where(f=>f.UserId == "2").ToList()
                },
                new UserProfile{
                    Id="3",
                    BirthDate=new DateTime(1996,12,25),
                    Name="Alina",
                    LastName="Koshevaya",
                    Sex=Sex.Female,
                    CityId=3,
                    City =cities[2],
                    Languages=languages.Where(l=>l.Id==1).ToList(),
                    Email="www.al.kshv@mail.ru",
                    SentMessages=messages.Where(m=>m.FromUserId=="3").ToList(),
                    Friends=friends.Where(f=>f.UserId == "3").ToList()
                },
                new UserProfile{
                    Id="4",
                    BirthDate=new DateTime(1996,7,11),
                    Name="Vlad",
                    LastName="Dyshlyk",
                    Sex=Sex.Male,
                    CityId=4,
                    City = cities[3],
                    Languages=languages.Where(l=>l.Id==3).ToList(),
                    Email="www.vld@mail.ru",
                    SentMessages=messages.Where(m=>m.FromUserId=="4").ToList(),
                    Friends=friends.Where(f=>f.UserId == "4").ToList()
                }
            };

        }
    }
}

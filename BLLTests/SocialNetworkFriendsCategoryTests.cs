using DataLayer.Interfaces;
using NUnit.Framework;
using Moq;
using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using System;
using DataLayer.Entities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace BLLTests
{
    [TestFixture]
    public class SocialNetworkFriendsCategoryTests
    {
        private IMapper Mapper { get; set; }
        SocialNetworkFunctionalityUser socialNetwork;
        List<UserProfile> userProfiles;
        List<Friend> friends;
        List<City> cities;
        List<Country> countries;
        List<Language> languages;
        List<UserMessage> messages;

        public TestContext testContext;
        //Mock<ISocialNetwork> moqSocialNetwork;
        //Mock<ILocalization> moqLocalization;
        //#region SocialNetwork Repos` moqs
        //Mock<IFriendRepository> moqFriendRepository;
        //Mock<IUserMessageRepository> moqUserMessageRepository;
        //Mock<IUserProfileRepository> moqUserProfileRepository;
        //#endregion
        //#region Localization Repos` moqs
        //Mock<ICountryRepository> moqCountryRepository;
        //Mock<ICityRepository> moqCityRepository;
        //Mock<ILanguageRepository> moqLanguageRepository;
        //#endregion
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
            MoqConfigurateRepositories();
            MoqConfigurateUoW();
            socialNetwork = new SocialNetworkFunctionalityUser(userId, moqSocialNetwork.Object, moqLocalization.Object, () => new DateTime(2011, 11, 11));

        }
        [Test]
        public async void FriendsCategory_Add_AsExpected()
        {
            var moqFriendRepository = new Mock<IFriendRepository>();
            moqFriendRepository.Setup(m => m.Add(It.IsAny<Friend>())).Returns<Friend>(res => res);
            var moqSocialNetwork = new Mock<ISocialNetwork>();
            moqSocialNetwork.Setup(m => m.Friends).Returns(moqFriendRepository.Object);
            string userToAddId = "userToAddId";
            Friend friend = new Friend
            {
                AddedDate = time,
                RequestDate = time,
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
        public async void FriendsCategory_Get_GetFriendsOfCuurentUser()
        {
            var moqFriendRepository = new Mock<IFriendRepository>();
            moqFriendRepository.Setup(m => m.GetAll()).Returns(friends.AsQueryable());
            var moqSocialNetwork = new Mock<ISocialNetwork>();
            moqSocialNetwork.Setup(m => m.Friends.GetAll()).Returns(moqFriendRepository.Object);

            var result = await socialNetwork.Friends.GetFriendsAsync();
            var expected = Mapper.Map<ICollection<UserProfile>, List<UserProfileDTO>>(userProfiles.Where(u=>u.Id=="2").ToList());
            var e = result[0];//.Friends.ToList()[0];
            var r = expected[0];
            if(result.SequenceEqual(expected))
            CollectionAssert.AreEqual(expected, result);
        }
        [Test]
        public void FriendsCategory_Confirm_AddsAndUpdatesExecuted()
        {
            socialNetwork.Id = "2";
            Friend friend = friends.Where(f => f.UserId == "1"  && f.FriendId == socialNetwork.Id).First();
            moqFriendRepository.Setup(m => m.GetFriend(It.Is<string>(v=>v=="1"), It.Is<string>(v=>v==socialNetwork.Id))).Returns(friend);

            Friend expectedConfirmed = new Friend
            {
                UserId = socialNetwork.Id,
                FriendId = "1",
                RequestUserId = friend.RequestUserId,
                Confirmed = true,
                ConfirmDate = socialNetwork.Now(),
                Deleted = friend.Deleted,
                DeleteDate = friend.DeleteDate,
                RequestDate = friend.RequestDate
            };
            var result = socialNetwork.Friends.Confirm("1");
            var expected = Mapper.Map<FriendDTO>(expectedConfirmed);

            moqFriendRepository.Verify(x => x.Update(It.IsAny<Friend>()));
            moqFriendRepository.Verify(x => x.Add(It.IsAny<Friend>()));
            Assert.AreEqual(expected, result);
        }

        private void MoqConfigurateUoW()
        {
            #region UnitOfWorks` Moqs
            #region SocialNetwork.UoW
            moqSocialNetwork = new Mock<ISocialNetwork>();
            moqSocialNetwork.Setup(m => m.GetFriendRepository()).Returns(moqFriendRepository.Object);
            moqSocialNetwork.Setup(m => m.GetUserMessageRepository()).Returns(moqUserMessageRepository.Object);
            moqSocialNetwork.Setup(m => m.GetUserProfileRepository()).Returns(moqUserProfileRepository.Object);

            moqSocialNetwork.Setup(m => m.Add(It.IsAny<Entity>())).Returns<Entity>(res => entity);
            moqSocialNetwork.Setup(m => m.AddRange(It.IsAny<IEnumerable<Entity>>())).Returns<IEnumerable<Entity>>(res => res);
            moqSocialNetwork.Setup(m => m.Commit());
            moqSocialNetwork.Setup(m => m.Remove(It.IsAny<Entity>()));
            moqSocialNetwork.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<Entity>>()));
            #endregion
            #region Localization.UoW
            moqLocalization = new Mock<ILocalization>();
            moqLocalization.Setup(m => m.GetCityRepository()).Returns<ICityRepository>(v => moqCityRepository.Object);
            moqLocalization.Setup(m => m.GetCountryRepository()).Returns<ICountryRepository>(v => moqCountryRepository.Object);
            moqLocalization.Setup(m => m.GetLanguageRepository()).Returns<ILanguageRepository>(v => moqLanguageRepository.Object);

            moqLocalization.Setup(m => m.Add(It.IsAny<Entity>())).Returns<Entity>(res => entity);
            moqLocalization.Setup(m => m.AddRange(It.IsAny<IEnumerable<Entity>>())).Returns<IEnumerable<Entity>>(res => res);
            moqLocalization.Setup(m => m.Commit());
            moqLocalization.Setup(m => m.Remove(It.IsAny<Entity>()));
            moqLocalization.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<Entity>>()));
            #endregion
            #endregion


        }
        //Firstly configurate Repositories` moqs
        private void MoqConfigurateRepositories()
        {
            #region SocialNetwork.UoW
            #region FriendRepository
            moqFriendRepository = new Mock<IFriendRepository>();
            moqFriendRepository.Setup(m => m.Add(It.IsAny<Friend>())).Returns<Friend>(res => res);
            moqFriendRepository.Setup(m => m.Get(It.IsAny<int>())).Returns<Friend>(res => res);
            moqFriendRepository.Setup(m => m.GetAllByUserId(It.IsAny<string>())).Returns(friends.Where(f=>f.UserId=="1").AsQueryable());
            moqFriendRepository.Setup(m => m.GetFriend(It.IsAny<string>(), It.IsAny<string>())).Returns(friends.Where(f => f.UserId == "1" && f.FriendId == "2").First());
            #endregion
            #region UserMessageRepository
            moqUserMessageRepository = new Mock<IUserMessageRepository>();
            moqUserMessageRepository.Setup(m => m.Add(It.IsAny<UserMessage>())).Returns<UserMessage>(res => res);
            moqUserMessageRepository.Setup(m => m.GetAll()).Returns(messages.AsQueryable());
            moqUserMessageRepository.Setup(m => m.GetAllByUserId(It.IsAny<string>())).Returns(messages.Where(u=>u.FromUserId=="1").AsQueryable());
            moqUserMessageRepository.Setup(m => m.Update(It.IsAny<UserMessage>())).Returns<UserMessage>(res => res);

            #endregion
            #region UserProfileRepository
            moqUserProfileRepository = new Mock<IUserProfileRepository>();
            moqUserProfileRepository.Setup(m => m.Add(It.IsAny<UserProfile>())).Returns<UserProfile>(res => res);
            moqUserProfileRepository.Setup(m => m.GetAll()).Returns(userProfiles.AsQueryable());
            moqUserProfileRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(userProfiles[0]);
            #endregion
            #endregion
            #region Localization.UoW
            #region CityRepository
            moqCityRepository = new Mock<ICityRepository>();
            moqCityRepository.Setup(m => m.Add(It.IsAny<City>())).Returns<City>(res => res);
            moqCityRepository.Setup(m => m.GetAll()).Returns(cities.AsQueryable());
            #endregion
            #region CountryRepository

            #endregion
            #region LanguageRepository

            #endregion
            #endregion


            //IQueryable<UserProfile> querableUserProfiles = userProfiles.AsQueryable();
            //moqFriendRepository = new Mock<IFriendRepository>();
            //moqFriendRepository.Setup(m => m.Add(It.IsAny<Friend>())).Returns<Friend>(res => res);
            //moqUserProfileRepository = new Mock<IUserProfileRepository>();
            //moqUserProfileRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(new UserProfile()/*It.IsAny<UserProfile>()*/);
            //moqUserProfileRepository.Setup(m => m.GetAll()).Returns(querableUserProfiles);
            //moqUserMessageRepository = new Mock<IUserMessageRepository>();
        }

        private void InitTestData()
        {
            UserProfile user1 = new UserProfile
            {
                Id = "1",
                BirthDate = new DateTime(1996, 7, 11),
                Name = "Paul",
                LastName = "Sokolov",
                Sex = Sex.male,
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
                Sex = Sex.male,
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
                Sex = Sex.female,
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
                Sex = Sex.male,
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
                    Sex=Sex.male,
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
                    Sex=Sex.male,
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
                    Sex=Sex.female,
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
                    Sex=Sex.male,
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

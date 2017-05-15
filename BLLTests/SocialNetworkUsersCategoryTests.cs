using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using Moq;
using NUnit.Framework;

namespace BLLTests
{
    [TestFixture]
    class SocialNetworkUsersCategoryTests
    {
        private IMapper Mapper { get; set; }
        Func<DateTime> NowAction => () => new DateTime(2017, 11, 11);

        private DateTime Now => NowAction();

        [OneTimeSetUp]
        public void Init()
        {
            Mapper = CustomMapper.Configurate();
        }

        [Test]
        public async Task GetUserByPublicId_GetByPublicIdAsync_AsExpected()
        {
            var expected = new UserProfile()
            {
                Id = "2",
                PublicId = 2,
                Name = "Ruslan"
            };
            var data = new List<UserProfile>
            {
                new UserProfile()
                {
                    Id = "1",
                    PublicId = 1,
                    Name = "Paul"
                },
                expected
            };

            var mockUserProfilesSet = TestAsyncQueryManger.MoqSetCreator(data.AsQueryable());

            var mockUserProfileRepository = new Mock<IUserProfileRepository>();
            mockUserProfileRepository.Setup(m => m.GetAll()).Returns(mockUserProfilesSet.Object);

            var mockSocialNetwork = new Mock<ISocialNetwork>();
            mockSocialNetwork.Setup(m => m.UserProfiles).Returns(mockUserProfileRepository.Object);

            var snManger = new SocialNetworkManager("1", mockSocialNetwork.Object, null, NowAction);

            var result = await snManger.Users.GetByPublicIdAsync(expected.PublicId);

            Assert.AreEqual(Mapper.Map<UserProfileDTO>(expected), result);
        }

        [Test]
        public async Task SearchUsers18yo_SearchAsync_AsExpected()
        {
           
            var users = new List<UserProfile>()
            {
                new UserProfile()
                {
                    Id = "1",
                    Name = "Paul",
                    BirthDate = new DateTime(1996,11,1)
                },
                new UserProfile()
                {
                    Id = "2",
                    Name = "Ruslan",
                    BirthDate = new DateTime(2000,11,1)
                },
                new UserProfile()
                {
                    Id = "3",
                    Name = "Vasya",
                    BirthDate = new DateTime(1999,11,1)
                }
            };
            var expected = users.Where(u => (Now.Year - u.BirthDate.Value.Year) >= 18);

            var mockUserProfilesSet = TestAsyncQueryManger.MoqSetCreator(users.AsQueryable());
            var mockUserProfileRepository = new Mock<IUserProfileRepository>();
            mockUserProfileRepository.Setup(m => m.GetAll()).Returns(mockUserProfilesSet.Object);
            var mockSocialNetwork = new Mock<ISocialNetwork>();
            mockSocialNetwork.Setup(m => m.UserProfiles).Returns(mockUserProfileRepository.Object);
            var soc = new SocialNetworkManager("1", mockSocialNetwork.Object, null, NowAction);

            var result = await soc.Users.SearchAsync(ageFrom: 18);

            Assert.AreEqual(Mapper.Map<List<UserProfileDTO>>(expected),result);
        }
        [Test]
        public async Task SearchUsersFrom18yoTo20_SearchAsync_AsExpected()
        {

            var users = new List<UserProfile>()
            {
                new UserProfile()
                {
                    Id = "1",
                    Name = "Paul",
                    BirthDate = new DateTime(1996,11,1)
                },
                new UserProfile()
                {
                    Id = "2",
                    Name = "Ruslan",
                    BirthDate = new DateTime(2000,11,1)
                },
                new UserProfile()
                {
                    Id = "3",
                    Name = "Vasya",
                    BirthDate = new DateTime(1999,11,1)
                }
            };
            var expected = users.Where(u => (Now.Year - u.BirthDate.Value.Year) >= 18 &&
                                            (Now.Year - u.BirthDate.Value.Year) <= 20);

            var mockUserProfilesSet = TestAsyncQueryManger.MoqSetCreator(users.AsQueryable());
            var mockUserProfileRepository = new Mock<IUserProfileRepository>();
            mockUserProfileRepository.Setup(m => m.GetAll()).Returns(mockUserProfilesSet.Object);
            var mockSocialNetwork = new Mock<ISocialNetwork>();
            mockSocialNetwork.Setup(m => m.UserProfiles).Returns(mockUserProfileRepository.Object);
            var soc = new SocialNetworkManager("1", mockSocialNetwork.Object, null, NowAction);

            var result = await soc.Users.SearchAsync(ageFrom: 18, ageTo:20);

            Assert.AreEqual(Mapper.Map<List<UserProfileDTO>>(expected), result);
        }
        [Test]
        public async Task SearchUsersFromCertainCountry_SearchAsync_AsExpected()
        {

            var users = new List<UserProfile>()
            {
                new UserProfile()
                {
                    Id = "1",
                    Name = "Paul",
                    City = new City{ Id = 1, CountryId = 1}
                },
                new UserProfile()
                {
                    Id = "2",
                    Name = "Ruslan",
                    City = new City{ Id = 2, CountryId = 1}
                },
                new UserProfile()
                {
                    Id = "3",
                    Name = "Vasya",
                    City = new City{ Id = 1, CountryId = 2}
                }
            };
            var expected = users.Where(u => u.City.CountryId == 1);

            var mockUserProfilesSet = TestAsyncQueryManger.MoqSetCreator(users.AsQueryable());
            var mockUserProfileRepository = new Mock<IUserProfileRepository>();
            mockUserProfileRepository.Setup(m => m.GetAll()).Returns(mockUserProfilesSet.Object);
            var mockSocialNetwork = new Mock<ISocialNetwork>();
            mockSocialNetwork.Setup(m => m.UserProfiles).Returns(mockUserProfileRepository.Object);
            var soc = new SocialNetworkManager("1", mockSocialNetwork.Object, null, NowAction);

            var result = await soc.Users.SearchAsync(countryId: 1);

            Assert.AreEqual(Mapper.Map<List<UserProfileDTO>>(expected), result);
        }

        [Test]
        public async Task GetCountriesThatHaveUsers_GetCountriesWithUsersAsync_ExpectedToGet2Countries()
        {
            var expected = new List<Country>()
            {
                new Country(){Id = 1, Name = "Ukraine"},
                new Country(){Id = 2, Name = "Russia"},
                new Country(){Id = 3, Name = "USA"}
            };
            var users = new List<UserProfile>()
            {
                new UserProfile()
                {
                    Id = "1",
                    Name = "Paul",
                    City = new City{ Id = 1, Country = new Country(){Id = 1, Name = "Ukraine"}}
                },
                new UserProfile()
                {
                    Id = "2",
                    Name = "Ruslan",
                    City = new City{ Id = 2, Country = new Country(){Id = 2, Name = "Russia"}}
                },
                new UserProfile()
                {
                    Id = "3",
                    Name = "Vasya",
                    City = new City{ Id = 1, Country = new Country(){Id = 3, Name = "USA"}}
                }
            };

            var mockUserProfilesSet = TestAsyncQueryManger.MoqSetCreator(users.AsQueryable());
            var mockUserProfileRepository = new Mock<IUserProfileRepository>();
            mockUserProfileRepository.Setup(m => m.GetAll()).Returns(mockUserProfilesSet.Object);
            var mockSocialNetwork = new Mock<ISocialNetwork>();
            mockSocialNetwork.Setup(m => m.UserProfiles).Returns(mockUserProfileRepository.Object);
            var soc = new SocialNetworkManager("1", mockSocialNetwork.Object, null, NowAction);
            var result = await soc.Users.GetCountriesWithUsersAsync();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(Mapper.Map<List<CountryDTO>>(expected), result);
        }
    }
}

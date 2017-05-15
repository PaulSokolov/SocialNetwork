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
    class SocialNetworkMessagesCategoryTests
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
        public async Task CountUnreadMessages_GetUnreadAsync_ExpectedToGet2UnreadMessages()
        {
            var expected = 2;
            var messages = new List<UserMessage>()
            {
                new UserMessage() {Id = 1, IsRead = true, ToUserId = "1"},
                new UserMessage() {Id = 2, IsRead = false, ToUserId = "1"},
                new UserMessage() {Id = 3, IsRead = false, ToUserId = "1"}
            };

            var mockUserMessagesSet = TestAsyncQueryManger.MoqSetCreator(messages.AsQueryable());
			var mockUserMessagesRepository = new Mock<IUserMessageRepository>();
            mockUserMessagesRepository.Setup(m => m.GetAll()).Returns(mockUserMessagesSet.Object);
			var mockSocialNetwork = new Mock<ISocialNetwork>();
            mockSocialNetwork.Setup(m => m.Messages).Returns(mockUserMessagesRepository.Object);

            var soc = new SocialNetworkManager("1", mockSocialNetwork.Object, null, NowAction);
            var result = await soc.Messages.GetUnreadAsync();

            Assert.AreEqual(expected, result);

        }

        [Test]
        public async Task GetDialogWithUser_GetDialogAsync_ExpectedToGet2Messages()
        {
            var expectedAmount = 3;
			
            var messages = new List<UserMessage>()
            {
                new UserMessage() {Id = 1, IsRead = true, ToUserId = "1", FromUserId = "2"},
                new UserMessage() {Id = 2, IsRead = false, ToUserId = "1", FromUserId = "2"},
                new UserMessage() {Id = 3, IsRead = false, ToUserId = "1", FromUserId = "3"},
                new UserMessage() {Id = 2, IsRead = false, ToUserId = "2", FromUserId = "1"}
            };
			var expectedMessages = new List<UserMessage>
			{
			    messages[0],
			    messages[1],
			    messages[3]
            };
            var mockUserMessagesSet = TestAsyncQueryManger.MoqSetCreator(messages.AsQueryable());
            var mockUserMessagesRepository = new Mock<IUserMessageRepository>();
            mockUserMessagesRepository.Setup(m => m.GetAll()).Returns(mockUserMessagesSet.Object);
            var mockSocialNetwork = new Mock<ISocialNetwork>();
            mockSocialNetwork.Setup(m => m.Messages).Returns(mockUserMessagesRepository.Object);

            var soc = new SocialNetworkManager("1", mockSocialNetwork.Object, null, NowAction);
            var result = await soc.Messages.GetDialogAsync("2");

			Assert.AreEqual(expectedAmount,result.Count);
            Assert.AreEqual(Mapper.Map<List<UserMessageDTO>>(expectedMessages), result);
        }
        [Test]
        public async Task FormDialogListWithLastMessages_GetLastMessagesAsync_ExpectedToGet3Messages()
        {
            var expectedAmount = 3;

            var messages = new List<UserMessage>()
            {
                new UserMessage() {Id = 1, IsRead = true, ToUserId = "1", FromUserId = "2", PostedDate = new DateTime(2017,11,10,12,12,12)},
                new UserMessage() {Id = 2, IsRead = false, ToUserId = "1", FromUserId = "2", PostedDate = new DateTime(2017,11,10,13,12,12)},
                new UserMessage() {Id = 3, IsRead = false, ToUserId = "1", FromUserId = "3", PostedDate = new DateTime(2017,11,10,12,12,12)},
                new UserMessage() {Id = 2, IsRead = false, ToUserId = "2", FromUserId = "1", PostedDate = new DateTime(2017,11,10,14,12,12)},
                new UserMessage() {Id = 1, IsRead = true, ToUserId = "1", FromUserId = "4", PostedDate = new DateTime(2017,11,10,13,12,12)},
                new UserMessage() {Id = 2, IsRead = false, ToUserId = "4", FromUserId = "1", PostedDate = new DateTime(2017,11,10,12,12,12)},
                new UserMessage() {Id = 3, IsRead = false, ToUserId = "1", FromUserId = "3", PostedDate = new DateTime(2017,11,10,11,12,12)},
                new UserMessage() {Id = 2, IsRead = false, ToUserId = "3", FromUserId = "1", PostedDate = new DateTime(2017,11,10,10,12,12)}
            };
            var expectedMessages = new List<UserMessage>
            {
                messages[3],
                messages[4],
                messages[6]
            };
            var mockUserMessagesSet = TestAsyncQueryManger.MoqSetCreator(messages.AsQueryable());
            var mockUserMessagesRepository = new Mock<IUserMessageRepository>();
            mockUserMessagesRepository.Setup(m => m.GetAll()).Returns(mockUserMessagesSet.Object);
            var mockSocialNetwork = new Mock<ISocialNetwork>();
            mockSocialNetwork.Setup(m => m.Messages).Returns(mockUserMessagesRepository.Object);

            var soc = new SocialNetworkManager("1", mockSocialNetwork.Object, null, NowAction);
            var result = await soc.Messages.GetLastMessagesAsync();

            Assert.AreEqual(expectedAmount, result.Count);
            Assert.AreEqual(Mapper.Map<List<UserMessageDTO>>(expectedMessages), result);
        }
    }
}
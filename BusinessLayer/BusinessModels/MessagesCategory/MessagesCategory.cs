using System.Collections.Generic;
using System.Linq;
using BusinessLayer.DTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using DataLayer.UnitOfWorks;

namespace BusinessLayer.BusinessModels
{
    public partial class SocialNetworkFunctionalityUser
    {
        public class MessagesCategory
        {
            private SocialNetworkFunctionalityUser _socialNetworkFunctionality;
            private ISocialNetwork _socialNetwork;

            public int UnRead
            {
                get => _socialNetwork.GetUserMessageRepository().GetAll()
                    .Where(m => m.ToUserId == _socialNetworkFunctionality.Id && m.IsRead == false).Count();
            }

            public MessagesCategory(SocialNetworkFunctionalityUser socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
                _socialNetwork = new SocialNetwork(_socialNetworkFunctionality._connection);
            }

            public List<UserMessageDTO> GetAllMessagesByUserId(string id)
            {
                var messages = _socialNetwork.GetUserMessageRepository().GetAllByUserId(id).ToList();

                return _socialNetworkFunctionality.Mapper.Map<List<UserMessage>, List<UserMessageDTO>>(messages);
            }

            public UserMessageDTO Moderate(long id, string body)
            {
                var mes = _socialNetwork.GetUserMessageRepository().Get(id);

                mes.Body = body;
                mes.ModifiedDate = _socialNetworkFunctionality.Now();

                var res = _socialNetwork.GetUserMessageRepository().Update(mes);
                _socialNetwork.Commit();

                return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(res);
            }

            public  bool Delete(long id)
            {
                var mes = _socialNetwork.GetUserMessageRepository().Get(id);

                if (mes != null)
                {
                    _socialNetwork.GetUserMessageRepository().Delete(mes);
                    _socialNetwork.Commit();

                    return true;
                }

                return false;
            }

            public UserMessageDTO Send(long recipientId, string body)
            {
                var recipient = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => u.PublicId == recipientId).Select(u => u.Id).FirstOrDefault();

                if (recipient == null)
                    throw new UserNotFoundException("There is no user with such publicId");

                var result = _socialNetwork.GetUserMessageRepository().Add(
                    new UserMessage
                    {
                        AddedDate = _socialNetworkFunctionality.Now(),
                        FromUserId = _socialNetworkFunctionality.Id,
                        ToUserId = recipient,
                        IsRead = false,
                        PostedDate = _socialNetworkFunctionality.Now(),
                        Body = body
                    });

                _socialNetwork.Commit();

                return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(result);
            }
            public List<IGrouping<string, UserMessageDTO>> GetAllDialogs()
            {
                var messages = _socialNetwork.GetUserMessageRepository().GetAll()
                    .Where(m => (m.FromUserId == _socialNetworkFunctionality.Id || m.ToUserId == _socialNetworkFunctionality.Id))//.ToList()
                    .GroupBy(d => d.FromUserId)
                    .ToList();

                return _socialNetworkFunctionality.Mapper.Map<List<IGrouping<string, UserMessage>>, List<IGrouping<string, UserMessageDTO>>>(messages);

            }
            public List<UserMessageDTO> GetDialog(string friendId)
            {
                var messages = _socialNetwork.GetUserMessageRepository().GetAll()
                    .Where(m => ((m.FromUserId == _socialNetworkFunctionality.Id && m.ToUserId == friendId) ||
                                                                                            (m.ToUserId == _socialNetworkFunctionality.Id && m.FromUserId == friendId)));

                return _socialNetworkFunctionality.Mapper.Map<IEnumerable<UserMessage>, List<UserMessageDTO>>(messages);
            }

            public List<UserMessageDTO> GetDialog(long publicFriendId)
            {
                var messages = _socialNetwork.GetUserMessageRepository().GetAll()
                    .Where(m => ((m.FromUser.PublicId == _socialNetworkFunctionality.Users.PublicId && m.ToUser.PublicId == publicFriendId) ||
                                                                                            (m.ToUser.PublicId == _socialNetworkFunctionality.Users.PublicId && m.FromUser.PublicId == publicFriendId))).OrderBy(m => m.PostedDate);

                return _socialNetworkFunctionality.Mapper.Map<IEnumerable<UserMessage>, List<UserMessageDTO>>(messages);
            }

            public UserMessageDTO GetLastMessage(string friendId)
            {
                UserMessage lastMessage = _socialNetwork.GetUserMessageRepository().GetAllByUserId(_socialNetworkFunctionality.Id)
                    .Where(m => m.ToUserId == friendId).OrderByDescending(m => m.PostedDate).FirstOrDefault();

                return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(lastMessage);
            }
            public List<UserMessageDTO> GetLastMessages()
            {
                var messages = _socialNetwork.GetUserMessageRepository().GetAll()
                    .Where(m => (m.FromUserId == _socialNetworkFunctionality.Id || m.ToUserId == _socialNetworkFunctionality.Id)).ToList()
                    .GroupBy(d => /*d.FromUserId*/new { d.FromUserId, d.ToUserId })
                    .Select(d => d.Select(m => m).LastOrDefault())
                    .ToList();

                var fromMe = messages.Where(m => m.FromUserId == _socialNetworkFunctionality.Id).ToList();
                var toMe = messages.Where(m => m.ToUserId == _socialNetworkFunctionality.Id).ToList();

                var dialogs = new List<UserMessage>();

                for (int i = 0; i < fromMe.Count; i++)
                {
                    bool same = false;
                    for (int j = 0; j < toMe.Count; j++)
                    {
                        if (fromMe[i].ToUserId == toMe[j].FromUserId)
                        {
                            same = true;
                            dialogs.Add(fromMe[i].PostedDate > toMe[j].PostedDate ? fromMe[i] : toMe[j]);
                        }
                    }
                    if (!same)
                        dialogs.Add(fromMe[i]);
                }

                return _socialNetworkFunctionality.Mapper.Map<List<UserMessage>, List<UserMessageDTO>>(dialogs);
            }

            public UserMessageDTO ReadMessage(long id)
            {
                UserMessage message = _socialNetwork.GetUserMessageRepository().Get(id);

                message.IsRead = true;

                var result = _socialNetwork.GetUserMessageRepository().Update(message);
                _socialNetwork.Commit();

                return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(result);
            }

            public void ReadMessages(IEnumerable<long> ids)
            {
                List<UserMessage> messages = _socialNetwork.GetUserMessageRepository().GetAll().Where(m => ids.Any(id => id == m.Id)).ToList();

                foreach (var message in messages)
                {
                    message.IsRead = true;
                    var result = _socialNetwork.GetUserMessageRepository().Update(message);
                }

                _socialNetwork.Commit();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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
            private readonly SocialNetworkFunctionalityUser _socialNetworkFunctionality;
            private readonly ISocialNetwork _socialNetwork;

            public int UnRead => _socialNetwork.Messages
                .GetAll().Count(m => m.ToUserId == _socialNetworkFunctionality.Id && m.IsRead == false);

            public MessagesCategory(SocialNetworkFunctionalityUser socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
                _socialNetwork = new SocialNetwork(_socialNetworkFunctionality._connection);
            }

            public async Task<int> GetUnreadAsync()
            {
                return await _socialNetwork.Messages
                    .GetAll().CountAsync(m => m.ToUserId == _socialNetworkFunctionality.Id && m.IsRead == false);
            }
            

            public async Task<List<UserMessageDTO>> GetAllMessagesByUserIdAsync(string id)
            {
                var messages = await _socialNetwork.Messages.GetAllSent(id).ToListAsync();

                return _socialNetworkFunctionality.Mapper.Map<List<UserMessage>, List<UserMessageDTO>>(messages);
            }

            public async Task<UserMessageDTO> ModerateAsync(long id, string body)
            {
                var mes = await _socialNetwork.Messages.GetAsync(id);

                mes.Body = body;
                mes.ModifiedDate = _socialNetworkFunctionality._now();

                var res = await _socialNetwork.Messages.UpdateAsync(mes);
                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(res);
            }

            public async Task<bool> DeleteAsync(long id)
            {
                var mes = await _socialNetwork.Messages.GetAsync(id);

                if (mes == null) return false;
                await _socialNetwork.Messages.DeleteAsync(mes);
                await _socialNetwork.CommitAsync();

                return true;
            }
            

            public async Task<UserMessageDTO> SendAsync(long recipientId, string body)
            {
                var recipient = await _socialNetwork.UserProfiles.GetAll().Where(u => u.PublicId == recipientId).Select(u => u.Id).FirstOrDefaultAsync();

                if (recipient == null)
                    throw new UserNotFoundException("There is no user with such publicId");

                var result = await _socialNetwork.Messages.AddAsync(
                    new UserMessage
                    {
                        AddedDate = _socialNetworkFunctionality._now(),
                        FromUserId = _socialNetworkFunctionality.Id,
                        ToUserId = recipient,
                        IsRead = false,
                        PostedDate = _socialNetworkFunctionality._now(),
                        Body = body
                    });

                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(result);
            }

            public async Task<List<IGrouping<string, UserMessageDTO>>> GetAllDialogsAsync()
            {
                var messages = await  _socialNetwork.Messages.GetAll().Where(m => (m.FromUserId == _socialNetworkFunctionality.Id || m.ToUserId == _socialNetworkFunctionality.Id))//.ToList()
                    .GroupBy(d => d.FromUserId)
                    .ToListAsync();

                return _socialNetworkFunctionality.Mapper.Map<List<IGrouping<string, UserMessage>>, List<IGrouping<string, UserMessageDTO>>>(messages);

            }

            public async Task<List<UserMessageDTO>> GetDialogAsync(string friendId)
            {
                var messages = await _socialNetwork.Messages.GetAll().Where(m => m.FromUserId == _socialNetworkFunctionality.Id && m.ToUserId == friendId ||
                                                                                             m.ToUserId == _socialNetworkFunctionality.Id && m.FromUserId == friendId).ToListAsync();

                return _socialNetworkFunctionality.Mapper.Map<IEnumerable<UserMessage>, List<UserMessageDTO>>(messages);
            }

            public async Task<List<UserMessageDTO>> GetDialogAsync(long publicFriendId)
            {
                var messages = await _socialNetwork.Messages.GetAll().Where(m => m.FromUser.PublicId == _socialNetworkFunctionality.Users.PublicId && m.ToUser.PublicId == publicFriendId ||
                                                                                             m.ToUser.PublicId == _socialNetworkFunctionality.Users.PublicId && m.FromUser.PublicId == publicFriendId).OrderBy(m => m.PostedDate).ToListAsync();

                return _socialNetworkFunctionality.Mapper.Map<IEnumerable<UserMessage>, List<UserMessageDTO>>(messages);
            }

            public async Task<List<UserMessageDTO>> GetLastMessagesAsync()
            {
                var repository = _socialNetwork.Messages;

                var messages = (await repository.GetAll().Where(m => (m.FromUserId == _socialNetworkFunctionality.Id ||
                                                                      m.ToUserId == _socialNetworkFunctionality.Id))
                        .ToListAsync())
                    .GroupBy(d => new {d.FromUserId, d.ToUserId})
                    .Select(d => d.Select(m => m).LastOrDefault()).ToList();

                var fromMe = messages.Where(m => m.FromUserId == _socialNetworkFunctionality.Id).ToList();
                var toMe = messages.Where(m => m.ToUserId == _socialNetworkFunctionality.Id).ToList();

                var dialogs = new List<UserMessage>();

                Parallel.ForEach(fromMe, mesFromMe =>
                {
                    var same = false;
                    Parallel.ForEach(toMe, (mesToMe) =>
                    {
                        if (mesFromMe.ToUserId == mesToMe.FromUserId)
                        {
                            same = true;
                            dialogs.Add(mesFromMe.PostedDate > mesToMe.PostedDate ? mesFromMe : mesToMe);
                        }
                    });
                    if (!same)
                        dialogs.Add(mesFromMe);
                });

                return _socialNetworkFunctionality.Mapper.Map<List<UserMessage>, List<UserMessageDTO>>(dialogs);
            }

            public async Task<UserMessageDTO> ReadAsync(long id)
            {
                UserMessage message = await _socialNetwork.Messages.GetAsync(id);

                message.IsRead = true;

                var result = await _socialNetwork.Messages.UpdateAsync(message);
                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(result);
            }
            //public void ReadMessages(IEnumerable<long> ids)
            //{
            //    var messages = _socialNetwork.GetUserMessageRepository().GetAll().Where(m => ids.Any(id => id == m.Id)).ToList();

            //    foreach (var message in messages)
            //    {
            //        message.IsRead = true;
            //        _socialNetwork.GetUserMessageRepository().Update(message);
            //    }

            //    _socialNetwork.Commit();
            //}
        }
    }
}
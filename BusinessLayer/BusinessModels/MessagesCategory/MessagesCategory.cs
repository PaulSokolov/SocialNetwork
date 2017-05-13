using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
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
            private SemaphoreSlim _semophore;
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
                using (var context = new SocialNetwork(_socialNetworkFunctionality._connection))
                {
                    return await context.Messages
                        .GetAll().CountAsync(m => m.ToUserId == _socialNetworkFunctionality.Id && m.IsRead == false);
                }
            }

            public async Task<UserMessageDTO> GetAsync(long id)
            {
                var mes = await _socialNetwork.Messages.GetAsync(id);
                if (mes == null)
                    throw new Exception();
                return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(mes);
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

                //Parallel.ForEach(fromMe, mesFromMe =>
                //{
                //    var same = false;
                //    Parallel.ForEach(toMe, (mesToMe) =>
                //    {
                //        UserMessage mesToAdd = null;
                //        if (mesFromMe.ToUserId == mesToMe.FromUserId)
                //        {
                //            same = true;

                //            lock (dialogs)
                //            {
                //                mesToAdd = mesFromMe.PostedDate > mesToMe.PostedDate ? mesFromMe : mesToMe;
                //                if (!dialogs.Contains(mesToAdd))
                //                    dialogs.Add(mesFromMe.PostedDate > mesToMe.PostedDate ? mesFromMe : mesToMe);

                //            }
                //        }
                //        if ( !dialogs.Contains(mesToMe) || !dialogs.Contains(mesFromMe))
                //            lock (dialogs)
                //            {
                //                dialogs.Add(mesToMe);
                //            }
                //    });
                //    if (!same)
                //        lock (dialogs)
                //        {
                //            dialogs.Add(mesFromMe);
                //        }
                //});
                List<Dialog> dialogLastmes = fromMe.AsParallel().Select(userMessage => new Dialog {FromMe = userMessage}).ToList();
                foreach (var lastMessagesInDialog in dialogLastmes)
                {
                    var fromUserId = lastMessagesInDialog.FromMe.FromUserId;
                    var toUserId = lastMessagesInDialog.FromMe.ToUserId;
                    var toMeItem = toMe.AsParallel().FirstOrDefault(m => m.FromUserId == toUserId && m.ToUserId == fromUserId);
                    lastMessagesInDialog.ToMe = toMeItem;
                    if (toMeItem != null)
                        toMe.Remove(toMeItem);
                }
                dialogLastmes.AddRange(toMe.Select(m => new Dialog {ToMe = m}));
                var messagess = dialogLastmes.AsParallel().Where(m=> m.ToMe != null && m.FromMe != null).Select(m => m.FromMe.PostedDate > m.ToMe.PostedDate ? m.FromMe : m.ToMe).ToList();
                messagess.AddRange(dialogLastmes.AsParallel().Where(m => m.ToMe == null).Select(m => m.FromMe));
                messagess.AddRange(dialogLastmes.AsParallel().Where(m => m.FromMe == null).Select(m => m.ToMe));
                dialogs = messagess;
                return _socialNetworkFunctionality.Mapper.Map<List<UserMessage>, List<UserMessageDTO>>(dialogs.ToList());
            }
            
            class Dialog
            {
                public UserMessage FromMe { get; set; }
                public UserMessage ToMe { get; set; }
            }
            public UserMessageDTO Read(long id)
            {
                using (var context = new SocialNetwork(_socialNetworkFunctionality._connection))
                {


                    UserMessage message = context.Messages.Get(id);
                    if (message.FromUserId == _socialNetworkFunctionality.Id)
                        return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(message);
                    message.IsRead = true;
                    //mes.Result.IsRead = true;
                    var result = context.Messages.Update(message);
                    context.Commit();


                    return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(result);
                }
            }

            public async Task<UserMessageDTO> ReadAsync(long id)
            {
                UserMessage message = await _socialNetwork.Messages.GetAsync(id);
                if(message.FromUserId == _socialNetworkFunctionality.Id && message.IsRead == true)
                    return _socialNetworkFunctionality.Mapper.Map<UserMessageDTO>(message);
                message.IsRead = true;
                //mes.Result.IsRead = true;
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
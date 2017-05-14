using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using DataLayer.UnitOfWorks;

namespace BusinessLayer.BusinessModels
{
    class Dialog
    {
        public UserMessage FromMe { get; set; }
        public UserMessage ToMe { get; set; }
    }
    public partial class SocialNetworkFunctionalityUser
    {

        public class MessagesCategory
        {
            private readonly SocialNetworkFunctionalityUser _socialNetworkFunctionality;
            private SemaphoreSlim Semaphore => _socialNetworkFunctionality._semaphore;
            private IMapper Mapper => _socialNetworkFunctionality._mapper;
            private string CurrentUserId => _socialNetworkFunctionality.Id;
            private DateTime Now => _socialNetworkFunctionality._now();
            private ISocialNetwork SocialNetwork => _socialNetworkFunctionality._socialNetwork ??
                                                    (_socialNetworkFunctionality._socialNetwork =
                                                        new SocialNetwork(Connection));

            public int UnRead => SocialNetwork.Messages
                .GetAll().Count(m => m.ToUserId == CurrentUserId && m.IsRead == false);

            public MessagesCategory(SocialNetworkFunctionalityUser socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
            }

            public async Task<int> GetUnreadAsync()
            {
                if (Semaphore.CurrentCount == Threads)
                {
                    await Semaphore.WaitAsync();
                    var res = await SocialNetwork.Messages
                        .GetAll().CountAsync(m => m.ToUserId == CurrentUserId && m.IsRead == false);
                    Semaphore.Release();
                    return res;
                }
                using (var context = new SocialNetwork(Connection))
                {
                    return await context.Messages
                        .GetAll().CountAsync(m => m.ToUserId == CurrentUserId && m.IsRead == false);
                }
            }

            public async Task<UserMessageDTO> GetAsync(long id)
            {
                await Semaphore.WaitAsync();

                var mes = await SocialNetwork.Messages.GetAsync(id);
                if (mes == null)
                    throw new Exception();

                Semaphore.Release();

                return Mapper.Map<UserMessageDTO>(mes);
            }

            public async Task<List<UserMessageDTO>> GetAllMessagesByUserIdAsync(string id)
            {
                await Semaphore.WaitAsync();

                var messages = await SocialNetwork.Messages.GetAllSent(id).ToListAsync();

                Semaphore.Release();

                return Mapper.Map<List<UserMessage>, List<UserMessageDTO>>(messages);
            }

            public async Task<UserMessageDTO> ModerateAsync(long id, string body)
            {
                await Semaphore.WaitAsync();

                var mes = await SocialNetwork.Messages.GetAsync(id);

                mes.Body = body;
                mes.ModifiedDate = Now;

                var res = await SocialNetwork.Messages.UpdateAsync(mes);
                await SocialNetwork.CommitAsync();

                Semaphore.Release();

                return Mapper.Map<UserMessageDTO>(res);
            }

            public async Task<bool> DeleteAsync(long id)
            {
                await Semaphore.WaitAsync();

                var mes = await SocialNetwork.Messages.GetAsync(id);

                if (mes == null) return false;
                await SocialNetwork.Messages.DeleteAsync(mes);
                await SocialNetwork.CommitAsync();

                Semaphore.Release();

                return true;
            }
            

            public async Task<UserMessageDTO> SendAsync(long recipientId, string body)
            {
                await Semaphore.WaitAsync();

                var recipient = await SocialNetwork.UserProfiles.GetAll().Where(u => u.PublicId == recipientId).Select(u => u.Id).FirstOrDefaultAsync();

                if (recipient == null)
                    throw new UserNotFoundException("There is no user with such publicId");

                var result = await SocialNetwork.Messages.AddAsync(
                    new UserMessage
                    {
                        AddedDate = Now,
                        FromUserId = CurrentUserId,
                        ToUserId = recipient,
                        IsRead = false,
                        PostedDate = Now,
                        Body = body
                    });

                await SocialNetwork.CommitAsync();

                Semaphore.Release();

                return Mapper.Map<UserMessageDTO>(result);
            }

            public async Task<List<IGrouping<string, UserMessageDTO>>> GetAllDialogsAsync()
            {
                var messages = await  SocialNetwork.Messages.GetAll().Where(m => (m.FromUserId == CurrentUserId || m.ToUserId == CurrentUserId))//.ToList()
                    .GroupBy(d => d.FromUserId)
                    .ToListAsync();

                return Mapper.Map<List<IGrouping<string, UserMessage>>, List<IGrouping<string, UserMessageDTO>>>(messages);

            }

            public async Task<List<UserMessageDTO>> GetDialogAsync(string friendId)
            {
                var messages = await SocialNetwork.Messages.GetAll().Where(m => m.FromUserId == CurrentUserId && m.ToUserId == friendId ||
                                                                                             m.ToUserId == CurrentUserId && m.FromUserId == friendId).ToListAsync();

                return Mapper.Map<IEnumerable<UserMessage>, List<UserMessageDTO>>(messages);
            }

            public async Task<List<UserMessageDTO>> GetDialogAsync(long publicFriendId, int lastIndex)
            {
                await Semaphore.WaitAsync();

                var currentUserPublicId = await _socialNetworkFunctionality.Users.GetPublicIdAsync();

                var messages = await SocialNetwork.Messages.GetAll().Where(
                        m => m.FromUser.PublicId == currentUserPublicId &&
                             m.ToUser.PublicId == publicFriendId ||
                             m.ToUser.PublicId == currentUserPublicId &&
                             m.FromUser.PublicId == publicFriendId).OrderByDescending(m => m.PostedDate).Skip(lastIndex)
                    .Take(10).ToListAsync();

                Semaphore.Release();

                return Mapper.Map<IEnumerable<UserMessage>, List<UserMessageDTO>>(messages);
            }

            public async Task<List<UserMessageDTO>> GetLastMessagesAsync()
            {
                await Semaphore.WaitAsync();

                var repository = SocialNetwork.Messages;

                var messages = (await repository.GetAll().Where(m => (m.FromUserId == CurrentUserId ||
                                                                      m.ToUserId == CurrentUserId))
                        .ToListAsync())
                    .GroupBy(d => new {d.FromUserId, d.ToUserId})
                    .Select(d => d.Select(m => m).LastOrDefault()).ToList();

                Semaphore.Release();

                var fromMe = messages.Where(m => m.FromUserId == CurrentUserId).ToList();
                var toMe = messages.Where(m => m.ToUserId == CurrentUserId).ToList();

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
                var dialogs = dialogLastmes.AsParallel().Where(m=> m.ToMe != null && m.FromMe != null).Select(m => m.FromMe.PostedDate > m.ToMe.PostedDate ? m.FromMe : m.ToMe).ToList();
                dialogs.AddRange(dialogLastmes.AsParallel().Where(m => m.ToMe == null).Select(m => m.FromMe));
                dialogs.AddRange(dialogLastmes.AsParallel().Where(m => m.FromMe == null).Select(m => m.ToMe));
                
                return Mapper.Map<List<UserMessage>, List<UserMessageDTO>>(dialogs.ToList());
            }
            
            
            public UserMessageDTO Read(long id)
            {
                if (Semaphore.CurrentCount == Threads)
                {
                    Semaphore.Wait();
                    UserMessage message = SocialNetwork.Messages.Get(id);
                    if (message.FromUserId == CurrentUserId)
                        return Mapper.Map<UserMessageDTO>(message);
                    message.IsRead = true;
                    //mes.Result.IsRead = true;
                    var result = SocialNetwork.Messages.Update(message);
                    SocialNetwork.Commit();

                    Semaphore.Release();

                    return Mapper.Map<UserMessageDTO>(result);
                }
                using (var context = new SocialNetwork(Connection))
                {


                    UserMessage message = context.Messages.Get(id);
                    if (message.FromUserId == CurrentUserId)
                        return Mapper.Map<UserMessageDTO>(message);
                    message.IsRead = true;
                    //mes.Result.IsRead = true;
                    var result = context.Messages.Update(message);
                    context.Commit();


                    return Mapper.Map<UserMessageDTO>(result);
                }
            }

            public async Task<UserMessageDTO> ReadAsync(long id)
            {
                await Semaphore.WaitAsync();

                UserMessage message = await SocialNetwork.Messages.GetAsync(id);
                if(message.FromUserId == CurrentUserId && message.IsRead == true)
                    return Mapper.Map<UserMessageDTO>(message);
                message.IsRead = true;
                //mes.Result.IsRead = true;
                var result = await SocialNetwork.Messages.UpdateAsync(message);
                await SocialNetwork.CommitAsync();

                Semaphore.Release();

                return Mapper.Map<UserMessageDTO>(result);

            }
        }
    }
}
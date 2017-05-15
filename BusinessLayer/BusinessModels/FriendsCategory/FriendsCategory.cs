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
    public partial class SocialNetworkFunctionalityUser
    {
        public partial class FriendsCategory
        {
            #region Private fields
            private FriendCounters _counters;
            private readonly SocialNetworkFunctionalityUser _socialNetworkFunctionality;

            private SemaphoreSlim Semaphore => _socialNetworkFunctionality._semaphore;
            private IMapper Mapper => _socialNetworkFunctionality._mapper;
            private string CurrentUserId => _socialNetworkFunctionality.Id;
            private DateTime Now => _socialNetworkFunctionality._now();
            private ISocialNetwork SocialNetwork => _socialNetworkFunctionality._socialNetwork ??
                                                    (_socialNetworkFunctionality._socialNetwork =
                                                        new SocialNetwork(Connection));
            #endregion

            public FriendCounters Counters => _counters ?? (_counters = new FriendCounters(this));

            public FriendsCategory(SocialNetworkFunctionalityUser socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
            }

            public async Task<FriendDTO> AddAsync(string userToAddId)
            {
                var user = await SocialNetwork.UserProfiles.GetAsync(userToAddId);

                if (user == null)
                    throw new UserNotFoundException("There is no such user.");

                var friend = await SocialNetwork.Friends
                    .AddAsync(new Friend
                    {
                        AddedDate = Now,
                        RequestDate = Now,
                        RequestUserId = CurrentUserId,
                        FriendId = userToAddId,
                        Confirmed = false,
                        UserId = CurrentUserId
                    });

                await SocialNetwork.CommitAsync();

                return Mapper.Map<Friend, FriendDTO>(friend);
            }

            public async Task<FriendDTO> AddAsync(long userToAddPublicId)
            {
                await Semaphore.WaitAsync();

                var user = await SocialNetwork.UserProfiles.GetAll().FirstOrDefaultAsync(u => u.PublicId == userToAddPublicId);

                if (user == null)
                    throw new UserNotFoundException("There is no such user.");

                var friendRepository = SocialNetwork.Friends;

                var friend = await friendRepository.AddAsync(new Friend
                {
                    AddedDate = Now,
                    RequestDate = Now,
                    RequestUserId = CurrentUserId,
                    FriendId = user.Id,
                    Confirmed = false,
                    UserId = CurrentUserId
                });

                await friendRepository.AddAsync(
                    new Friend
                    {
                        AddedDate = Now,
                        RequestDate = Now,
                        RequestUserId = CurrentUserId,
                        FriendId = CurrentUserId,
                        Confirmed = false,
                        UserId = user.Id
                    });
                await SocialNetwork.CommitAsync();

                Semaphore.Release();

                return Mapper.Map<Friend, FriendDTO>(friend);
            }

            public async Task<FriendDTO> ConfirmAsync(long userToAddPublicId)
            {
                await Semaphore.WaitAsync();

                var userToAddId = await SocialNetwork.UserProfiles.GetAll().Where(u => u.PublicId == userToAddPublicId).Select(u => u.Id).FirstOrDefaultAsync();
                

                if (userToAddId == null)
                    throw new UserNotFoundException();

                var friendRepository = SocialNetwork.Friends;

                Friend friend = await friendRepository.GetFriend(userToAddId, CurrentUserId);

                friend.ConfirmDate = Now;
                friend.Confirmed = true;
                friend.Deleted = false;

                var task = SocialNetwork.Friends.UpdateAsync(friend);

                Friend confirmedFriend = await friendRepository.GetFriend(CurrentUserId, userToAddId);

                confirmedFriend.ConfirmDate = Now;
                confirmedFriend.Confirmed = true;
                confirmedFriend.Deleted = false;

                var res = SocialNetwork.Friends.UpdateAsync(confirmedFriend);

                await Task.WhenAll(task, res);
                await SocialNetwork.CommitAsync();

                Semaphore.Release();

                return Mapper.Map<FriendDTO>(res.Result);
            }

            public async Task<FriendDTO> DeleteAsync(long userToDeletePublicId)
            {
                await Semaphore.WaitAsync();

                var userToDelete = await SocialNetwork.UserProfiles.GetAll()
                    .Where(u => u.PublicId == userToDeletePublicId).Select(u => u.Id).FirstOrDefaultAsync();


                if (userToDelete == null)
                    throw new UserNotFoundException();
                var friendRepository = SocialNetwork.Friends;
                Friend friend = await friendRepository.GetFriend(CurrentUserId, userToDelete);

                if (friend == null)
                    throw new UserNotFoundException("There is no user to delete");

                var deletedDate = Now;

                friend.Confirmed = true;
                friend.DeleteDate = deletedDate;
                friend.Deleted = true;

                var resTask = SocialNetwork.Friends.UpdateAsync(friend);

                Friend deletedFriend = await friendRepository.GetFriend(userToDelete, CurrentUserId);

                deletedFriend.Confirmed = true;
                deletedFriend.Deleted = true;
                deletedFriend.DeleteDate = deletedDate;

                var updateTask = SocialNetwork.Friends.UpdateAsync(deletedFriend);
                await Task.WhenAll(resTask, updateTask);
                await SocialNetwork.CommitAsync();

                Semaphore.Release();

                return Mapper.Map<FriendDTO>(resTask.Result);
            }

            public async Task UnsubscribeAsync(long unsubscribeId)
            {
                await Semaphore.WaitAsync();

                var userToDelete = await SocialNetwork.UserProfiles.GetAll().FirstOrDefaultAsync(u => u.PublicId == unsubscribeId);

                if (userToDelete == null)
                    throw new UserNotFoundException();
                var friendRepository = SocialNetwork.Friends;

                Friend friend = await friendRepository.GetFriend(CurrentUserId, userToDelete.Id);

                if (friend == null)
                    throw new UserNotFoundException("There is no user to delete");


                await friendRepository.DeleteAsync(friend);

                Friend deletedFriend =
                    await friendRepository.GetFriend(userToDelete.Id, CurrentUserId);

                if (deletedFriend != null)
                    await friendRepository.DeleteAsync(deletedFriend);

                await SocialNetwork.CommitAsync();

                Semaphore.Release();
            }

            public async Task<ICollection<UserProfileDTO>> GetFriendsAsync()
            {
                List<UserProfile> friends;
                if (Semaphore.CurrentCount == Threads)
                {
                    await Semaphore.WaitAsync();
                    var query = SocialNetwork.Friends.GetAll().Where(f => f.UserId == CurrentUserId)
                        .Where(u => u.Confirmed && u.Deleted == false).Select(u => u.FriendId);

                    friends = await SocialNetwork.UserProfiles.GetAll().Where(u => query.Any(f => f == u.Id)).ToListAsync();

                    Semaphore.Release();
                    return Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
                using (var context = new SocialNetwork(Connection))
                {
                    var query = context.Friends.GetAll().Where(f => f.UserId == CurrentUserId)
                        .Where(u => u.Confirmed && u.Deleted == false).Select(u => u.FriendId);

                    friends = await context.UserProfiles.GetAll().Where(u => query.Any(f => f == u.Id)).ToListAsync();


                    return Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
            }

            public async Task<ICollection<UserProfileDTO>> GetFriendsAsync(int count)
            {
                List<UserProfile> friends;
                if (Semaphore.CurrentCount == Threads)
                {
                    await Semaphore.WaitAsync();
                    var query = SocialNetwork.Friends.GetAll().Where(f => f.UserId == CurrentUserId)
                        .Where(u => u.Confirmed && u.Deleted == false).Select(u => u.FriendId);

                    friends = await SocialNetwork.UserProfiles.GetAll().Where(u => query.Any(f => f == u.Id)).Take(count).ToListAsync();

                    Semaphore.Release();
                    return Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
                using (var context = new SocialNetwork(Connection))
                {
                    var query = context.Friends.GetAll().Where(f => f.UserId == CurrentUserId)
                        .Where(u => u.Confirmed && u.Deleted == false).Select(u => u.FriendId);

                    friends = await context.UserProfiles.GetAll().Where(u => query.Any(f => f == u.Id)).Take(count).ToListAsync();


                    return Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
            }

            public async Task<ICollection<UserProfileDTO>> GetFollowedAsync()
            {
                List<UserProfile> friends;
                if (Semaphore.CurrentCount == Threads)
                {
                    await Semaphore.WaitAsync();
                    var query = SocialNetwork.Friends.GetAll()
                        .Where(f => f.UserId == CurrentUserId)
                        .Where(f => (f.Confirmed == false || f.Deleted) &&
                                    f.RequestUserId == CurrentUserId).Select(u => u.FriendId);
                    friends = await SocialNetwork.UserProfiles.GetAll()
                        .Where(u => query.Any(f => f == u.Id)).ToListAsync();
                    Semaphore.Release();
                    return Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
                using (var context = new SocialNetwork(Connection))
                {
                    var query = context.Friends.GetAll()
                        .Where(f => f.UserId == CurrentUserId)
                        .Where(f => (f.Confirmed == false || f.Deleted) &&
                                    f.RequestUserId == CurrentUserId).Select(u => u.FriendId);
                    friends = await context.UserProfiles.GetAll()
                        .Where(u => query.Any(f => f == u.Id)).ToListAsync();

                    return Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
            }

            public async Task<ICollection<UserProfileDTO>> GetFollowersAsync()
            {
                List<UserProfile> friends;
                if (Semaphore.CurrentCount == Threads)
                {
                    await Semaphore.WaitAsync();
                    var query = SocialNetwork.Friends.GetAll()
                        .Where(f => f.FriendId == CurrentUserId)
                        .Where(f => (f.Confirmed == false || f.Deleted) &&
                                    f.RequestUserId != CurrentUserId).Select(u => u.UserId);
                    friends = await SocialNetwork.UserProfiles.GetAll()
                        .Where(u => query.Any(f => f == u.Id)).ToListAsync();
                    Semaphore.Release();
                    return Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
                using (var context = new SocialNetwork(Connection))
                {
                    var query = context.Friends.GetAll()
                        .Where(f => f.FriendId == CurrentUserId)
                        .Where(f => (f.Confirmed == false || f.Deleted) &&
                                    f.RequestUserId != CurrentUserId).Select(u => u.UserId);
                    friends = await context.UserProfiles.GetAll()
                        .Where(u => query.Any(f => f == u.Id)).ToListAsync();

                    return Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
            }
        }
    }
}

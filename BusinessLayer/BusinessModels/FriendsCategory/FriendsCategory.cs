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
        public partial class FriendsCategory
        {
            #region Private fields
            private readonly ISocialNetwork _socialNetwork;
            private readonly SocialNetworkFunctionalityUser _socialNetworkFunctionality;
            private FriendCounters _counters;
            #endregion

            public FriendCounters Counters => _counters ?? (_counters = new FriendCounters(this));

            public FriendsCategory(SocialNetworkFunctionalityUser socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
                _socialNetwork = _socialNetworkFunctionality._socialNetworkConnection;
                _socialNetwork = new SocialNetwork(_socialNetworkFunctionality._connection);
            }

            public async Task<FriendDTO> AddAsync(string userToAddId)
            {
                var user = await _socialNetwork.UserProfiles.GetAsync(userToAddId);

                if (user == null)
                    throw new UserNotFoundException("There is no such user.");

                var friend = await _socialNetwork.Friends
                    .AddAsync(new Friend
                    {
                        AddedDate = _socialNetworkFunctionality._now(),
                        RequestDate = _socialNetworkFunctionality._now(),
                        RequestUserId = _socialNetworkFunctionality.Id,
                        FriendId = userToAddId,
                        Confirmed = false,
                        UserId = _socialNetworkFunctionality.Id
                    });

                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<Friend, FriendDTO>(friend);
            }

            public async Task<FriendDTO> AddAsync(long userToAddPublicId)
            {
                var user = await _socialNetwork.UserProfiles.GetAll().FirstOrDefaultAsync(u => u.PublicId == userToAddPublicId);

                if (user == null)
                    throw new UserNotFoundException("There is no such user.");

                var friendRepository = _socialNetwork.Friends;

                var friend = await friendRepository.AddAsync(new Friend
                {
                    AddedDate = _socialNetworkFunctionality._now(),
                    RequestDate = _socialNetworkFunctionality._now(),
                    RequestUserId = _socialNetworkFunctionality.Id,
                    FriendId = user.Id,
                    Confirmed = false,
                    UserId = _socialNetworkFunctionality.Id
                });

                var task = await friendRepository.AddAsync(
                    new Friend
                    {
                        AddedDate = _socialNetworkFunctionality._now(),
                        RequestDate = _socialNetworkFunctionality._now(),
                        RequestUserId = _socialNetworkFunctionality.Id,
                        FriendId = _socialNetworkFunctionality.Id,
                        Confirmed = false,
                        UserId = user.Id
                    });
                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<Friend, FriendDTO>(friend);
            }

            public async Task<FriendDTO> ConfirmAsync(long userToAddPublicId)
            {
                var userToAddId = await _socialNetwork.UserProfiles.GetAll().Where(u => u.PublicId == userToAddPublicId).Select(u => u.Id).FirstOrDefaultAsync();

                if (userToAddId == null)
                    throw new UserNotFoundException();
                var friendRepository = _socialNetwork.Friends;
                Friend friend = await friendRepository.GetFriend(userToAddId, _socialNetworkFunctionality.Id);

                friend.ConfirmDate = _socialNetworkFunctionality._now();
                friend.Confirmed = true;
                friend.Deleted = false;

                var task = _socialNetwork.Friends.UpdateAsync(friend);

                Friend confirmedFriend = await friendRepository.GetFriend(_socialNetworkFunctionality.Id, userToAddId);

                confirmedFriend.ConfirmDate = _socialNetworkFunctionality._now();
                confirmedFriend.Confirmed = true;
                confirmedFriend.Deleted = false;

                var res = _socialNetwork.Friends.UpdateAsync(confirmedFriend);

                await Task.WhenAll(task, res);
                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<FriendDTO>(res.Result);
            }

            public async Task<FriendDTO> DeleteAsync(long userToDeletePublicId)
            {
                var userToDelete = await _socialNetwork.UserProfiles.GetAll()
                    .Where(u => u.PublicId == userToDeletePublicId).Select(u => u.Id).FirstOrDefaultAsync();

                if (userToDelete == null)
                    throw new UserNotFoundException();
                var friendRepository = _socialNetwork.Friends;
                Friend friend = await friendRepository.GetFriend(_socialNetworkFunctionality.Id, userToDelete);

                if (friend == null)
                    throw new UserNotFoundException("There is no user to delete");

                var deletedDate = _socialNetworkFunctionality._now();

                friend.Confirmed = true;
                friend.DeleteDate = deletedDate;
                friend.Deleted = true;

                var resTask = _socialNetwork.Friends.UpdateAsync(friend);

                Friend deletedFriend = await friendRepository.GetFriend(userToDelete, _socialNetworkFunctionality.Id);

                deletedFriend.Confirmed = true;
                deletedFriend.Deleted = true;
                deletedFriend.DeleteDate = deletedDate;

                var updateTask = _socialNetwork.Friends.UpdateAsync(deletedFriend);
                await Task.WhenAll(resTask, updateTask);
                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<FriendDTO>(resTask.Result);
            }

            public async Task UnsubscribeAsync(long unsubscribeId)
            {
                var userToDelete = await _socialNetwork.UserProfiles.GetAll().FirstOrDefaultAsync(u => u.PublicId == unsubscribeId);

                if (userToDelete == null)
                    throw new UserNotFoundException();
                var friendRepository = _socialNetwork.Friends;
                Friend friend = await friendRepository.GetFriend(_socialNetworkFunctionality.Id, userToDelete.Id);

                if (friend == null)
                    throw new UserNotFoundException("There is no user to delete");


                await friendRepository.DeleteAsync(friend);

                Friend deletedFriend =
                    await friendRepository.GetFriend(userToDelete.Id, _socialNetworkFunctionality.Id);

                if (deletedFriend != null)
                    await friendRepository.DeleteAsync(deletedFriend);

                await _socialNetwork.CommitAsync();
            }

            public async Task<ICollection<UserProfileDTO>> GetFriendsAsync()
            {
                List<UserProfile> friends = new List<UserProfile>();
                using (var context = new SocialNetwork(_socialNetworkFunctionality._connection))
                {
                    var query = context.Friends.GetAll().Where(f => f.UserId == _socialNetworkFunctionality.Id)
                        .Where(u => u.Confirmed && u.Deleted == false).Select(u => u.FriendId);
                    friends = await context.UserProfiles.GetAll().Where(u => query.Any(f => f == u.Id)).ToListAsync();


                    return _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
            }

            public async Task<ICollection<UserProfileDTO>> GetFollowedAsync()
            {
                List<UserProfile> friends = new List<UserProfile>();
                using (var context = new SocialNetwork(_socialNetworkFunctionality._connection))
                {
                    var query = context.Friends.GetAll()
                        .Where(f => f.UserId == _socialNetworkFunctionality.Id)
                        .Where(f => (f.Confirmed == false || f.Deleted) &&
                                    f.RequestUserId == _socialNetworkFunctionality.Id).Select(u => u.FriendId);
                    friends = await context.UserProfiles.GetAll()
                        .Where(u => query.Any(f => f == u.Id)).ToListAsync();

                    return _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
            }

            public async Task<ICollection<UserProfileDTO>> GetFollowersAsync()
            {
                List<UserProfile> friends = new List<UserProfile>();
                using (var context = new SocialNetwork(_socialNetworkFunctionality._connection))
                {
                    var query = context.Friends.GetAll()
                        .Where(f => f.FriendId == _socialNetworkFunctionality.Id)
                        .Where(f => (f.Confirmed == false || f.Deleted) &&
                                    f.RequestUserId != _socialNetworkFunctionality.Id).Select(u => u.UserId);
                    friends = await context.UserProfiles.GetAll()
                        .Where(u => query.Any(f => f == u.Id)).ToListAsync();

                    return _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
                }
            }
        }
    }
}

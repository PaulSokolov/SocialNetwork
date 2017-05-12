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

            public FriendDTO Add(string userToAddId)
            {
                var user = _socialNetwork.GetUserProfileRepository().GetUserProfile(userToAddId);

                if (user == null)
                    throw new UserNotFoundException("There is no such user.");

                var friend = _socialNetwork.GetFriendRepository()
                    .Add(new Friend
                    {
                        AddedDate = _socialNetworkFunctionality._now(),
                        RequestDate = _socialNetworkFunctionality._now(),
                        RequestUserId = _socialNetworkFunctionality.Id,
                        FriendId = userToAddId,
                        Confirmed = false,
                        UserId = _socialNetworkFunctionality.Id
                    });

                _socialNetwork.Commit();

                return _socialNetworkFunctionality.Mapper.Map<Friend, FriendDTO>(friend);
            }

            public async Task<FriendDTO> AddAsync(string userToAddId)
            {
                var user = await (await _socialNetwork.GetUserProfileRepositoryAsync()).GetUserProfileAsync(userToAddId);

                if (user == null)
                    throw new UserNotFoundException("There is no such user.");

                var friend = await (await _socialNetwork.GetFriendRepositoryAsync())
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

            public FriendDTO Add(long userToAddPublicId)
            {
                var user = _socialNetwork.GetUserProfileRepository().GetAll().FirstOrDefault(u => u.PublicId == userToAddPublicId);

                if (user == null)
                    throw new UserNotFoundException("There is no such user.");

                var friend = _socialNetwork.GetFriendRepository()
                    .Add(new Friend
                    {
                        AddedDate = _socialNetworkFunctionality._now(),
                        RequestDate = _socialNetworkFunctionality._now(),
                        RequestUserId = _socialNetworkFunctionality.Id,
                        FriendId = user.Id,
                        Confirmed = false,
                        UserId = _socialNetworkFunctionality.Id
                    });

                _socialNetwork.GetFriendRepository().Add(
                    new Friend
                    {
                        AddedDate = _socialNetworkFunctionality._now(),
                        RequestDate = _socialNetworkFunctionality._now(),
                        RequestUserId = _socialNetworkFunctionality.Id,
                        FriendId = _socialNetworkFunctionality.Id,
                        Confirmed = false,
                        UserId = user.Id
                    });

                _socialNetwork.Commit();

                return _socialNetworkFunctionality.Mapper.Map<Friend, FriendDTO>(friend);
            }
            
            public async Task<FriendDTO> AddAsync(long userToAddPublicId)
            {
                var user = await (await _socialNetwork.GetUserProfileRepositoryAsync()).GetAll().FirstOrDefaultAsync(u => u.PublicId == userToAddPublicId);

                if (user == null)
                    throw new UserNotFoundException("There is no such user.");

                var frindRepository = await _socialNetwork.GetFriendRepositoryAsync();

                var friend = frindRepository.AddAsync(new Friend
                    {
                        AddedDate = _socialNetworkFunctionality._now(),
                        RequestDate = _socialNetworkFunctionality._now(),
                        RequestUserId = _socialNetworkFunctionality.Id,
                        FriendId = user.Id,
                        Confirmed = false,
                        UserId = _socialNetworkFunctionality.Id
                    });

                frindRepository.AddAsync(
                    new Friend
                    {
                        AddedDate = _socialNetworkFunctionality._now(),
                        RequestDate = _socialNetworkFunctionality._now(),
                        RequestUserId = _socialNetworkFunctionality.Id,
                        FriendId = _socialNetworkFunctionality.Id,
                        Confirmed = false,
                        UserId = user.Id
                    });
                Task.WaitAll();
                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<Friend, FriendDTO>(friend.Result);
            }

            public FriendDTO Confirm(string userToAddId)
            {
                Friend friend = _socialNetwork.GetFriendRepository().GetFriend(userToAddId, _socialNetworkFunctionality.Id);

                friend.ConfirmDate = _socialNetworkFunctionality._now();
                friend.Confirmed = true;

                _socialNetwork.GetFriendRepository().Update(friend);

                var res = _socialNetwork.GetFriendRepository().Add(
                    new Friend
                    {
                        UserId = _socialNetworkFunctionality.Id,
                        FriendId = userToAddId,
                        RequestUserId = friend.RequestUserId,
                        Confirmed = friend.Confirmed,
                        ConfirmDate = friend.ConfirmDate,
                        Deleted = friend.Deleted,
                        DeleteDate = friend.DeleteDate,
                        RequestDate = friend.RequestDate
                    });

                _socialNetwork.Commit();

                return _socialNetworkFunctionality.Mapper.Map<FriendDTO>(res);
            }



            public FriendDTO Confirm(long userToAddPublicId)
            {
                var userToAddId = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => u.PublicId == userToAddPublicId).Select(u => u.Id).FirstOrDefault();

                if (userToAddId == null)
                    throw new UserNotFoundException();
                Friend friend = _socialNetwork.GetFriendRepository().GetFriend(userToAddId, _socialNetworkFunctionality.Id);

                friend.ConfirmDate = _socialNetworkFunctionality._now();
                friend.Confirmed = true;
                friend.Deleted = false;

                _socialNetwork.GetFriendRepository().Update(friend);

                Friend confirmedFriend = _socialNetwork.GetFriendRepository().GetFriend(_socialNetworkFunctionality.Id, userToAddId);

                confirmedFriend.ConfirmDate = _socialNetworkFunctionality._now();
                confirmedFriend.Confirmed = true;
                confirmedFriend.Deleted = false;

                var res = _socialNetwork.GetFriendRepository().Update(confirmedFriend);

                _socialNetwork.Commit();
                
                return _socialNetworkFunctionality.Mapper.Map<FriendDTO>(res);
            }

            public async Task<FriendDTO> ConfirmAsync(long userToAddPublicId)
            {
                var userToAddId = await (await _socialNetwork.GetUserProfileRepositoryAsync()).GetAll().Where(u => u.PublicId == userToAddPublicId).Select(u => u.Id).FirstOrDefaultAsync();

                if (userToAddId == null)
                    throw new UserNotFoundException();
                var friendRepository = await _socialNetwork.GetFriendRepositoryAsync();
                Friend friend = await friendRepository.GetFriendAsync(userToAddId, _socialNetworkFunctionality.Id);

                friend.ConfirmDate = _socialNetworkFunctionality._now();
                friend.Confirmed = true;
                friend.Deleted = false;

                _socialNetwork.GetFriendRepository().UpdateAsync(friend);

                Friend confirmedFriend = await friendRepository.GetFriendAsync(_socialNetworkFunctionality.Id, userToAddId);

                confirmedFriend.ConfirmDate = _socialNetworkFunctionality._now();
                confirmedFriend.Confirmed = true;
                confirmedFriend.Deleted = false;

                var res = _socialNetwork.GetFriendRepository().UpdateAsync(confirmedFriend);

                Task.WaitAll();
                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<FriendDTO>(res.Result);
            }

            public void Delete(string userToDeleteId)
            {
                Friend friend = _socialNetwork.GetFriendRepository().GetFriend(_socialNetworkFunctionality.Id, userToDeleteId);

                if (friend == null)
                    throw new UserNotFoundException("There is no user to delete");

                friend.Confirmed = true;
                friend.DeleteDate = _socialNetworkFunctionality._now();
                friend.Deleted = true;

                _socialNetwork.GetFriendRepository().Update(friend);

                var deletedFriend = _socialNetwork.GetFriendRepository().GetFriend(userToDeleteId, _socialNetworkFunctionality.Id);

                deletedFriend.Confirmed = friend.Confirmed;
                deletedFriend.Deleted = friend.Deleted;
                deletedFriend.DeleteDate = friend.DeleteDate;

                _socialNetwork.GetFriendRepository().Update(deletedFriend);
                _socialNetwork.Commit();
            }

            public FriendDTO Delete(long userToDeletePublicId)
            {
                var userToDelete = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => u.PublicId == userToDeletePublicId).Select(u => u.Id).FirstOrDefault();

                if (userToDelete == null)
                    throw new UserNotFoundException();

                Friend friend = _socialNetwork.GetFriendRepository().GetFriend(_socialNetworkFunctionality.Id, userToDelete);

                if (friend == null)
                    throw new UserNotFoundException("There is no user to delete");

                friend.Confirmed = true;
                friend.DeleteDate = _socialNetworkFunctionality._now();
                friend.Deleted = true;

                var res = _socialNetwork.GetFriendRepository().Update(friend);

                var deletedFriend = _socialNetwork.GetFriendRepository().GetFriend(userToDelete, _socialNetworkFunctionality.Id);

                deletedFriend.Confirmed = friend.Confirmed;
                deletedFriend.Deleted = friend.Deleted;
                deletedFriend.DeleteDate = friend.DeleteDate;

                _socialNetwork.GetFriendRepository().Update(deletedFriend);
                _socialNetwork.Commit();

                return _socialNetworkFunctionality.Mapper.Map<FriendDTO>(res);
            }

            public async Task<FriendDTO> DeleteAsync(long userToDeletePublicId)
            {
                var userToDelete = await (await _socialNetwork.GetUserProfileRepositoryAsync()).GetAll()
                    .Where(u => u.PublicId == userToDeletePublicId).Select(u => u.Id).FirstOrDefaultAsync();

                if (userToDelete == null)
                    throw new UserNotFoundException();
                var friendRepository = await _socialNetwork.GetFriendRepositoryAsync();
                Friend friend = await friendRepository.GetFriendAsync(_socialNetworkFunctionality.Id, userToDelete);

                if (friend == null)
                    throw new UserNotFoundException("There is no user to delete");

                var deletedDate = _socialNetworkFunctionality._now();

                friend.Confirmed = true;
                friend.DeleteDate = deletedDate;
                friend.Deleted = true;

                var resTask = _socialNetwork.GetFriendRepository().UpdateAsync(friend);

                Friend deletedFriend = await friendRepository.GetFriendAsync(userToDelete, _socialNetworkFunctionality.Id);

                deletedFriend.Confirmed = true;
                deletedFriend.Deleted = true;
                deletedFriend.DeleteDate = deletedDate;

                var updateTask = _socialNetwork.GetFriendRepository().UpdateAsync(deletedFriend);
                await Task.WhenAll(resTask, updateTask);
                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<FriendDTO>(resTask.Result);
            }

            public void Unsubscribe(long unsubscribeId)
            {
                var userToDelete = _socialNetwork.GetUserProfileRepository().GetAll().FirstOrDefault(u => u.PublicId == unsubscribeId);

                if (userToDelete == null)
                    throw new UserNotFoundException();

                Friend friend = _socialNetwork.GetFriendRepository().GetFriend(_socialNetworkFunctionality.Id, userToDelete.Id);

                if (friend == null)
                    throw new UserNotFoundException("There is no user to delete");


                _socialNetwork.GetFriendRepository().Delete(friend);

                var deletedFriend = _socialNetwork.GetFriendRepository().GetFriend(userToDelete.Id, _socialNetworkFunctionality.Id);

                if (deletedFriend != null)
                    _socialNetwork.GetFriendRepository().Delete(deletedFriend);

                _socialNetwork.Commit();
            }

            public async Task UnsubscribeAsync(long unsubscribeId)
            {
                var userToDelete = await (await _socialNetwork.GetUserProfileRepositoryAsync()).GetAll().FirstOrDefaultAsync(u => u.PublicId == unsubscribeId);

                if (userToDelete == null)
                    throw new UserNotFoundException();
                var friendRepository = await _socialNetwork.GetFriendRepositoryAsync();
                Friend friend = await friendRepository.GetFriendAsync(_socialNetworkFunctionality.Id, userToDelete.Id);

                if (friend == null)
                    throw new UserNotFoundException("There is no user to delete");


                friendRepository.DeleteAsync(friend);

                Friend deletedFriend =
                    await friendRepository.GetFriendAsync(userToDelete.Id, _socialNetworkFunctionality.Id);

                if (deletedFriend != null)
                    friendRepository.DeleteAsync(deletedFriend);
                Task.WaitAll();
                await _socialNetwork.CommitAsync();
            }

            public ICollection<UserProfileDTO> GetFriends()
            {
                var query = _socialNetwork.GetFriendRepository().GetAllByUserId(_socialNetworkFunctionality.Id).Where(u => u.Confirmed && u.Deleted == false).Select(u => u.FriendId);
                var friends = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => query.Any(f => f == u.Id)).ToList();

                return _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
            }

            public async Task<ICollection<UserProfileDTO>> GetFriendsAsync()
            {
                var query = (await _socialNetwork.GetFriendRepositoryAsync()).GetAllByUserId(_socialNetworkFunctionality.Id).Where(u => u.Confirmed && u.Deleted == false).Select(u => u.FriendId);
                var friends = await (await  _socialNetwork.GetUserProfileRepositoryAsync()).GetAll().Where(u => query.Any(f => f == u.Id)).ToListAsync();

                return _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
            }

            public ICollection<UserProfileDTO> GetFollowed()
            {
                var query = _socialNetwork.GetFriendRepository().GetAllByUserId(_socialNetworkFunctionality.Id)
                    .Where(f => (f.Confirmed == false || f.Deleted) &&
                                f.RequestUserId == _socialNetworkFunctionality.Id).Select(u => u.FriendId);
                var friends = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => query.Any(f => f == u.Id))
                    .ToList();

                return _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
            }

            public async Task<ICollection<UserProfileDTO>> GetFollowedAsync()
            {
                var query = (await _socialNetwork.GetFriendRepositoryAsync())
                    .GetAllByUserId(_socialNetworkFunctionality.Id)
                    .Where(f => (f.Confirmed == false || f.Deleted) &&
                                f.RequestUserId == _socialNetworkFunctionality.Id).Select(u => u.FriendId);
                var friends = await (await _socialNetwork.GetUserProfileRepositoryAsync()).GetAll()
                    .Where(u => query.Any(f => f == u.Id)).ToListAsync();

                return _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
            }

            public ICollection<UserProfileDTO> GetFollowers()
            {
                var query = _socialNetwork.GetFriendRepository().GetAll()
                    .Where(f => f.FriendId == _socialNetworkFunctionality.Id)
                    .Where(f => (f.Confirmed == false || f.Deleted) &&
                                f.RequestUserId != _socialNetworkFunctionality.Id).Select(u => u.UserId);
                var friends = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => query.Any(f => f == u.Id))
                    .ToList();

                return _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
            }

            public async Task<ICollection<UserProfileDTO>> GetFollowersAsync()
            {
                var query = (await _socialNetwork.GetFriendRepositoryAsync()).GetAll()
                    .Where(f => f.FriendId == _socialNetworkFunctionality.Id)
                    .Where(f => (f.Confirmed == false || f.Deleted) &&
                                f.RequestUserId != _socialNetworkFunctionality.Id).Select(u => u.UserId);
                var friends = await (await _socialNetwork.GetUserProfileRepositoryAsync()).GetAll()
                    .Where(u => query.Any(f => f == u.Id)).ToListAsync();

                return _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);
            }
        }
    }
}

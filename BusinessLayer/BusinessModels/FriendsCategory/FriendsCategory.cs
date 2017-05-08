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
        public partial class FriendsCategory
        {
            #region Private fields
            private ISocialNetwork _socialNetwork;
            private SocialNetworkFunctionalityUser _socialNetworkFunctionality;
            private FriendCounters _counters; 
            #endregion

            public FriendCounters Counters
            {
                get
                {
                    if (_counters == null)
                        _counters = new FriendCounters(this);

                    return _counters;
                }
            }

            public FriendsCategory(SocialNetworkFunctionalityUser socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
                _socialNetwork = _socialNetworkFunctionality._socialNetworkConnection;
                _socialNetwork = new SocialNetwork(_socialNetworkFunctionality._connection);
            }

            public FriendDTO Add(string userToAddId)
            {
                var user = _socialNetwork.GetUserProfileRepository().Get(userToAddId);

                if (user == null)
                    throw new UserNotFoundException("There is no such user.");

                var friend = _socialNetwork.GetFriendRepository()
                    .Add(new Friend
                    {
                        AddedDate = _socialNetworkFunctionality.Now(),
                        RequestDate = _socialNetworkFunctionality.Now(),
                        RequestUserId = _socialNetworkFunctionality.Id,
                        FriendId = userToAddId,
                        Confirmed = false,
                        UserId = _socialNetworkFunctionality.Id
                    });

                _socialNetwork.Commit();

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
                        AddedDate = _socialNetworkFunctionality.Now(),
                        RequestDate = _socialNetworkFunctionality.Now(),
                        RequestUserId = _socialNetworkFunctionality.Id,
                        FriendId = user.Id,
                        Confirmed = false,
                        UserId = _socialNetworkFunctionality.Id
                    });

                _socialNetwork.GetFriendRepository().Add(
                    new Friend
                    {
                        AddedDate = _socialNetworkFunctionality.Now(),
                        RequestDate = _socialNetworkFunctionality.Now(),
                        RequestUserId = _socialNetworkFunctionality.Id,
                        FriendId = _socialNetworkFunctionality.Id,
                        Confirmed = false,
                        UserId = user.Id
                    });

                _socialNetwork.Commit();

                return _socialNetworkFunctionality.Mapper.Map<Friend, FriendDTO>(friend);
            }

            public FriendDTO Confirm(string userToAddId)
            {
                Friend friend = _socialNetwork.GetFriendRepository().GetFriend(userToAddId, _socialNetworkFunctionality.Id);

                friend.ConfirmDate = _socialNetworkFunctionality.Now();
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

                friend.ConfirmDate = _socialNetworkFunctionality.Now();
                friend.Confirmed = true;
                friend.Deleted = false;

                _socialNetwork.GetFriendRepository().Update(friend);

                Friend confirmedFriend = _socialNetwork.GetFriendRepository().GetFriend(_socialNetworkFunctionality.Id, userToAddId);

                confirmedFriend.ConfirmDate = _socialNetworkFunctionality.Now();
                confirmedFriend.Confirmed = true;
                confirmedFriend.Deleted = false;

                var res = _socialNetwork.GetFriendRepository().Update(confirmedFriend);

                _socialNetwork.Commit();
                
                return _socialNetworkFunctionality.Mapper.Map<FriendDTO>(res);
            }
            public void Delete(string userToDeleteId)
            {
                Friend friend = _socialNetwork.GetFriendRepository().GetFriend(_socialNetworkFunctionality.Id, userToDeleteId);

                if (friend == null)
                    throw new UserNotFoundException("There is no user to delete");

                friend.Confirmed = true;
                friend.DeleteDate = _socialNetworkFunctionality.Now();
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
                friend.DeleteDate = _socialNetworkFunctionality.Now();
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

            public ICollection<UserProfileDTO> GetFriends()
            {
                var query = _socialNetwork.GetFriendRepository().GetAllByUserId(_socialNetworkFunctionality.Id.ToString()).Where(u => u.Confirmed == true && u.Deleted == false).Select(u => u.FriendId);
                var friends = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => query.Any(f => f == u.Id)).ToList();

                var users = _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);

                return users;
            }
            public ICollection<UserProfileDTO> GetFollowed()
            {
                var query = _socialNetwork.GetFriendRepository().GetAllByUserId(_socialNetworkFunctionality.Id.ToString()).Where(f => (f.Confirmed == false || f.Deleted == true) && f.RequestUserId == _socialNetworkFunctionality.Id).Select(u => u.FriendId);
                var friends = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => query.Any(f => f == u.Id)).ToList();

                var users = _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);

                return users;
            }
            public ICollection<UserProfileDTO> GetFollowers()
            {
                var query = _socialNetwork.GetFriendRepository().GetAll().Where(f => f.FriendId == _socialNetworkFunctionality.Id).Where(f => (f.Confirmed == false || f.Deleted == true) && f.RequestUserId != _socialNetworkFunctionality.Id).Select(u => u.UserId);
                var friends = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => query.Any(f => f == u.Id)).ToList();

                var users = _socialNetworkFunctionality.Mapper.Map<List<UserProfile>, List<UserProfileDTO>>(friends);

                return users;
            }
        }
    }
}

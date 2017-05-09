using System;
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
        public class UsersCategory
        {
            #region Private fields
            private SocialNetworkFunctionalityUser _socialNetworkFunctionality;
            private ISocialNetwork _socialNetwork;
            private string _avatar;
            private long? _publicId; 
            #endregion

            public string Avatar
            {
                get
                {
                    if (_avatar == null)
                        _avatar = _socialNetwork.GetUserProfileRepository().Get(_socialNetworkFunctionality.Id).Avatar;

                    return _avatar;
                }
            }
            public long PublicId
            {
                get
                {
                    _publicId = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => u.Id == _socialNetworkFunctionality.Id).Select(u => u.PublicId).FirstOrDefault();

                    if (_publicId == null)
                        throw new UserNotFoundException();

                    return (long)_publicId;
                }
            }

            public UsersCategory(SocialNetworkFunctionalityUser socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
                _socialNetwork = new SocialNetwork(_socialNetworkFunctionality._connection);
            }

            public UserProfileDTO Get(string id)
            {
                UserProfile userProfile = _socialNetwork.GetUserProfileRepository().Get(id);

                if (userProfile == null)
                    throw new UserNotFoundException("There is no such user.");

                return _socialNetworkFunctionality.Mapper.Map<UserProfile, UserProfileDTO>(userProfile);
            }

            public UserProfileDTO GetByPublicId(long publicId)
            {
                UserProfile userProfile = _socialNetwork.GetUserProfileRepository().GetAll().FirstOrDefault(u => u.PublicId == publicId);

                if (userProfile == null)
                    throw new UserNotFoundException("There is no such user.");

                return _socialNetworkFunctionality.Mapper.Map<UserProfile, UserProfileDTO>(userProfile);
            }

            public UserProfileDTO Update(UserProfileDTO user)
            {
                UserProfile up = _socialNetwork.GetUserProfileRepository().Get(user.Id);

                up.ModifiedDate = _socialNetworkFunctionality.Now();
                up.Avatar = up.Avatar;
                up.About = user.About;
                up.AboutIsHidden = user.AboutIsHidden;
                up.Activity = user.Activity;
                up.ActivityIsHidden = user.ActivityIsHidden;
                up.Address = user.Address;
                up.BirthDate = user.BirthDate;
                up.BirthDateIsHidden = user.BirthDateIsHidden;
                up.CityId = user.CityId;
                up.Email = user.Email;
                up.EmailIsHidden = user.EmailIsHidden;
                up.LastName = user.LastName;
                up.Name = user.LastName;
                up.Sex = _socialNetworkFunctionality.Mapper.Map<Sex>(user.Sex);

                up = _socialNetwork.GetUserProfileRepository().Update(up);
                _socialNetwork.Commit();

                return _socialNetworkFunctionality.Mapper.Map<UserProfileDTO>(up);
            }

            public List<UserProfileDTO> Search(string name)
            {
                var users = _socialNetwork.GetUserProfileRepository().GetAll().Where(u => u.Name.Contains(name) || u.LastName.Contains(name));

                return _socialNetworkFunctionality.Mapper.Map<List<UserProfileDTO>>(users);
            }

            public List<UserProfileDTO> Search(string search = null, int? ageFrom = null, int? ageTo = null, long? cityId = null, long? countryId = null, string activityConcurence = null, string aboutConcurence = null, int? sex = null, short? sort = null)
            {
                var time = _socialNetworkFunctionality.Now().Year;
                var query = _socialNetwork.GetUserProfileRepository().GetAll();

                activityConcurence = activityConcurence == string.Empty ? null : activityConcurence;
                aboutConcurence = aboutConcurence == string.Empty ? null : aboutConcurence;
                search = search == string.Empty ? null : search;

                if (search != null)
                    query = query.Where(u => u.Name.Contains(search) || u.LastName.Contains(search));                        
                if (ageFrom != null)
                    query = query.Where(u => (time - ((DateTime)u.BirthDate).Year) > ageFrom);
                if (ageTo != null)
                    query = query.Where(u => (time - ((DateTime)u.BirthDate).Year) < ageTo);
                if (countryId != null)
                    query = query.Where(u => u.City.CountryId == countryId);
                if (cityId != null)
                    query = query.Where(u => u.CityId == cityId);
                if (sex != null)
                    query = query.Where(u => u.Sex == (Sex)sex);
                if (sort != null)
                {
                    if (sort == 0)
                        query = query.OrderBy(u => u.ActivatedDate);
                    else if (sort == 1)
                        query = query.OrderByDescending(u => u.ActivatedDate);
                }

                var users = query.ToList();

                if (users.Count == 0)
                {
                    if (activityConcurence != null)
                    {
                        string[] activities = activityConcurence.Split(' ');
                        query = query.Where(u => activities.Any(s => u.Activity.Contains(s)));
                    }
                    if (aboutConcurence != null)
                    {
                        string[] about = aboutConcurence.Split(' ');
                        query = query.Where(u => about.Any(s => u.Activity.Contains(s)));
                    }
                    
                    users = query.ToList();
                }
                
                return _socialNetworkFunctionality.Mapper.Map<List<UserProfileDTO>>(users);
            }

            public List<CountryDTO> GetCountriesWithUsers()
            {
                var counties = _socialNetwork.GetUserProfileRepository().GetAll().GroupBy(u => u.City.Country).Select(s => s.Key);
                
                return _socialNetworkFunctionality.Mapper.Map<List<CountryDTO>>(counties);
            }


        }
    }
}
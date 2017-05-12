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
        public class UsersCategory
        {
            #region Private fields
            private readonly SocialNetworkFunctionalityUser _socialNetworkFunctionality;
            private readonly ISocialNetwork _socialNetwork;
            private string _avatar;
            private long? _publicId; 
            #endregion

            public string Avatar => _avatar ?? (_avatar = _socialNetwork.UserProfiles.Get(_socialNetworkFunctionality.Id).Avatar);

            public long PublicId
            {
                get
                {
                    _publicId = _socialNetwork.UserProfiles.GetAll().Where(u => u.Id == _socialNetworkFunctionality.Id).Select(u => u.PublicId).FirstOrDefault();

                    if (_publicId == null)
                        throw new UserNotFoundException();

                    return (long)_publicId;
                }
            }

            public async Task<string> GetAvatarAsync()
            {
                using (var context = new SocialNetwork(_socialNetworkFunctionality._connection))
                {
                    return _avatar ?? (_avatar = (await context.UserProfiles
                               .GetAsync(_socialNetworkFunctionality.Id)).Avatar);
                }
            }

            public async Task<long> GetPublicIdAsync()
            {
                using (var context = new SocialNetwork(_socialNetworkFunctionality._connection))
                {
                    _publicId = await context.UserProfiles.GetAll().Where(u => u.Id == _socialNetworkFunctionality.Id)
                        .Select(u => u.PublicId).FirstOrDefaultAsync();

                    if (_publicId == null)
                        throw new UserNotFoundException();

                    return (long) _publicId;
                }
            }

            public UsersCategory(SocialNetworkFunctionalityUser socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
                _socialNetwork = new SocialNetwork(_socialNetworkFunctionality._connection);
            }

            public async Task<UserProfileDTO> GetAsync(string id)
            {
                UserProfile userProfile = await _socialNetwork.UserProfiles.GetAsync(id);

                if (userProfile == null)
                    throw new UserNotFoundException("There is no such user.");

                return _socialNetworkFunctionality.Mapper.Map<UserProfile, UserProfileDTO>(userProfile);
            }

            public async Task<UserProfileDTO> GetByPublicIdAsync(long publicId)
            {
                UserProfile userProfile = await _socialNetwork.UserProfiles.GetAll().FirstOrDefaultAsync(u => u.PublicId == publicId);

                if (userProfile == null)
                    throw new UserNotFoundException("There is no such user.");

                return _socialNetworkFunctionality.Mapper.Map<UserProfile, UserProfileDTO>(userProfile);
            }

            public async Task<UserProfileDTO> UpdateAsync(UserProfileDTO user)
            {
                UserProfile up = await _socialNetwork.UserProfiles.GetAsync(user.Id);

                up.ModifiedDate = _socialNetworkFunctionality._now();
                up.Avatar = user.Avatar;
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
                up.Name = user.Name;
                up.Sex = _socialNetworkFunctionality.Mapper.Map<Sex>(user.Sex);

                up = await _socialNetwork.UserProfiles.UpdateAsync(up);
                await _socialNetwork.CommitAsync();

                return _socialNetworkFunctionality.Mapper.Map<UserProfileDTO>(up);
            }

            public async Task<List<UserProfileDTO>> SearchAsync(string name)
            {
                var users = await _socialNetwork.UserProfiles.GetAll().Where(u => u.Name.Contains(name) || u.LastName.Contains(name)).ToListAsync();

                return _socialNetworkFunctionality.Mapper.Map<List<UserProfileDTO>>(users);
            }
            
            public async Task<List<UserProfileDTO>> SearchAsync(string search = null, int? ageFrom = null, int? ageTo = null, long? cityId = null, long? countryId = null, string activityConcurence = null, string aboutConcurence = null, int? sex = null, short? sort = null)
            {
                var time = _socialNetworkFunctionality._now().Year;
                var query = _socialNetwork.UserProfiles.GetAll();

                activityConcurence = activityConcurence == string.Empty ? null : activityConcurence;
                aboutConcurence = aboutConcurence == string.Empty ? null : aboutConcurence;
                search = search == string.Empty ? null : search;

                if (search != null)
                    query = query.Where(u => u.Name.Contains(search) || u.LastName.Contains(search));
                if (ageFrom != null)
                    query = query.Where(u => time - ((DateTime)u.BirthDate).Year > ageFrom);
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
                    switch (sort)
                    {
                        case 0:
                            query = query.OrderBy(u => u.ActivatedDate);
                            break;
                        case 1:
                            query = query.OrderByDescending(u => u.ActivatedDate);
                            break;
                    }
                }

                var users = await query.ToListAsync();

                if (users.Count != 0) return _socialNetworkFunctionality.Mapper.Map<List<UserProfileDTO>>(users);

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

                users = await query.ToListAsync();

                return _socialNetworkFunctionality.Mapper.Map<List<UserProfileDTO>>(users);
            }

            public async Task<List<CountryDTO>> GetCountriesWithUsersAsync()
            {
                var counties = await _socialNetwork.UserProfiles.GetAll().GroupBy(u => u.City.Country).Select(s => s.Key).ToListAsync();

                return _socialNetworkFunctionality.Mapper.Map<List<CountryDTO>>(counties);
            }


        }
    }
}
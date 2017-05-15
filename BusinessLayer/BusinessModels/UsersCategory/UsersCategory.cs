using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.DTO;
using BusinessLayer.Interfaces;
using DataLayer.Entities;
using DataLayer.Interfaces;
using DataLayer.UnitOfWorks;

namespace BusinessLayer.BusinessModels
{
    public partial class SocialNetworkManager
    {
        public class UsersCategory: IUsersCategory
        {
            #region Private fields
            private readonly SocialNetworkManager _socialNetworkFunctionality;
            //private readonly ISocialNetwork _socialNetwork;
            private string _avatar;
            private long? _publicId;
            private SemaphoreSlim Semaphore => _socialNetworkFunctionality._semaphore;
            private IMapper Mapper => _socialNetworkFunctionality._mapper;
            private string CurrentUserId => _socialNetworkFunctionality.Id;
            private DateTime Now => _socialNetworkFunctionality._now();
            private ISocialNetwork SocialNetwork => _socialNetworkFunctionality._socialNetwork ??
                                                    (_socialNetworkFunctionality._socialNetwork =
                                                        new SocialNetwork(Connection));
            private ILocalization Localization => _socialNetworkFunctionality._localizationConnection ??
                                                  (_socialNetworkFunctionality._localizationConnection =
                                                      new Localization(Connection));
            #endregion


            public string Avatar => _avatar ?? (_avatar = SocialNetwork.UserProfiles.Get(CurrentUserId).Avatar);

            public long PublicId
            {
                get
                {
                    Semaphore.Wait();
                    _publicId = SocialNetwork.UserProfiles
                        .GetAll()
                        .Where(u => u.Id == CurrentUserId)
                        .Select(u => u.PublicId)
                        .FirstOrDefault();
                    Semaphore.Release();
                    if (_publicId == null)
                        throw new UserNotFoundException();

                    return (long)_publicId;
                }
            }

            public UsersCategory(SocialNetworkManager socialNetworkFunctionality)
            {
                _socialNetworkFunctionality = socialNetworkFunctionality;
                //_socialNetwork = new SocialNetwork(Connection);
            }

            public async Task<UserProfileDTO> AddLanguageAsync(long publicId, long languageId)
            {
                await Semaphore.WaitAsync();

                var languageToAdd = await SocialNetwork.Languages.GetAsync(languageId);
                var user = await SocialNetwork.UserProfiles.GetAsync(publicId);
                user.Languages.Add(languageToAdd);
                var res = await SocialNetwork.UserProfiles.UpdateAsync(user);
                await SocialNetwork.CommitAsync();

                Semaphore.Release();

                return Mapper.Map<UserProfileDTO>(res);
            }

            public async Task<string> GetAvatarAsync()
            {
                if (Semaphore.CurrentCount == Threads)
                {
                    await Semaphore.WaitAsync();
                    var res = _avatar ?? (_avatar = (await SocialNetwork.UserProfiles
                                  .GetAsync(CurrentUserId)).Avatar);
                    Semaphore.Release();
                    return res;
                }
                using (var context = new SocialNetwork(Connection))
                {
                    return _avatar ?? (_avatar = (await context.UserProfiles
                               .GetAsync(CurrentUserId)).Avatar);
                }
            }

            public async Task<long> GetPublicIdAsync()
            {
                if (Semaphore.CurrentCount == Threads)
                {
                    await Semaphore.WaitAsync();
                    _publicId = await SocialNetwork.UserProfiles.GetAll().Where(u => u.Id == CurrentUserId)
                        .Select(u => u.PublicId).FirstOrDefaultAsync();
                    Semaphore.Release();
                    if (_publicId == null)
                        throw new UserNotFoundException();

                    return (long)_publicId;
                }
                using (var context = new SocialNetwork(Connection))
                {
                    _publicId = await context.UserProfiles.GetAll().Where(u => u.Id == CurrentUserId)
                        .Select(u => u.PublicId).FirstOrDefaultAsync();

                    if (_publicId == null)
                        throw new UserNotFoundException();

                    return (long) _publicId;
                }
            }

            public async Task<UserProfileDTO> GetAsync(string id)
            {
                if (Semaphore.CurrentCount == Threads)
                {
                    await Semaphore.WaitAsync();
                    UserProfile userProfile = await SocialNetwork.UserProfiles.GetAsync(id);
                    Semaphore.Release();
                    if (userProfile == null)
                        throw new UserNotFoundException("There is no such user.");

                    return Mapper.Map<UserProfile, UserProfileDTO>(userProfile);
                }
                using (var context = new SocialNetwork(Connection))
                {
                    UserProfile userProfile = await context.UserProfiles.GetAsync(id);

                    if (userProfile == null)
                        throw new UserNotFoundException("There is no such user.");

                    return Mapper.Map<UserProfile, UserProfileDTO>(userProfile);
                }
               
            }

            public async Task<UserProfileDTO> GetByPublicIdAsync(long publicId)
            {
                await Semaphore.WaitAsync();

                UserProfile userProfile = await SocialNetwork.UserProfiles.GetAll().FirstOrDefaultAsync(u => u.PublicId == publicId);

                Semaphore.Release();

                if (userProfile == null)
                    throw new UserNotFoundException("There is no such user.");

                return Mapper.Map<UserProfile, UserProfileDTO>(userProfile);
            }

            public async Task<UserProfileDTO> UpdateAsync(UserProfileDTO user)
            {
                await Semaphore.WaitAsync();

                UserProfile up = await SocialNetwork.UserProfiles.GetAsync(user.Id);

                up.ModifiedDate = Now;
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
                up.Sex = Mapper.Map<Sex>(user.Sex);

                up = await SocialNetwork.UserProfiles.UpdateAsync(up);
                await SocialNetwork.CommitAsync();

                Semaphore.Release();

                return Mapper.Map<UserProfileDTO>(up);
            }

            public async Task<List<UserProfileDTO>> SearchAsync(string name)
            {
                await Semaphore.WaitAsync();

                var users = await SocialNetwork.UserProfiles.GetAll().Where(u => u.Name.Contains(name) || u.LastName.Contains(name)).ToListAsync();

                Semaphore.Release();

                return Mapper.Map<List<UserProfileDTO>>(users);
            }
            
            public async Task<List<UserProfileDTO>> SearchAsync(string search = null, int? ageFrom = null, int? ageTo = null, long? cityId = null,
                long? countryId = null, string activityConcurence = null, string aboutConcurence = null, int? sex = null, short? sort = 0, int? lastIndex = 0)
            {
                var time = Now.Year;
                var query = SocialNetwork.UserProfiles.GetAll();

                activityConcurence = activityConcurence == string.Empty ? null : activityConcurence;
                aboutConcurence = aboutConcurence == string.Empty ? null : aboutConcurence;
                search = search == string.Empty ? null : search;

                if (search != null)
                    query = query.Where(u => u.Name.Contains(search) || u.LastName.Contains(search));
                if (ageFrom != null)
                    query = query.Where(u => time - ((DateTime)u.BirthDate).Year >= ageFrom);
                if (ageTo != null)
                    query = query.Where(u => time - ((DateTime)u.BirthDate).Year <= ageTo);
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

                if (!lastIndex.HasValue)
                {
                    throw new UserNotFoundException();
                }

                await Semaphore.WaitAsync();

                var users = await query.Skip(lastIndex.Value).Take(10).ToListAsync();

                Semaphore.Release();

                if (users.Count != 0) return Mapper.Map<List<UserProfileDTO>>(users);

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

                await Semaphore.WaitAsync();

                users = await query.Skip(lastIndex.Value).Take(10).ToListAsync();

                Semaphore.Release();

                return Mapper.Map<List<UserProfileDTO>>(users);
            }

            public async Task<List<CountryDTO>> GetCountriesWithUsersAsync()
            {
                await Semaphore.WaitAsync();

                var counties = await SocialNetwork.UserProfiles.GetAll().GroupBy(u => u.City.Country).Select(s => s.Key).ToListAsync();

                Semaphore.Release();

                return Mapper.Map<List<CountryDTO>>(counties);
            }


        }
    }
}
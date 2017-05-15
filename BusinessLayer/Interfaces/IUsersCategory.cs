using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTO;

namespace BusinessLayer.Interfaces
{
    public interface IUsersCategory
    {
        string Avatar { get; }
        long PublicId { get; }
        Task<UserProfileDTO> AddLanguageAsync(long publicId, long languageId);
        Task<string> GetAvatarAsync();
        Task<long> GetPublicIdAsync();
        Task<UserProfileDTO> GetAsync(string id);
        Task<UserProfileDTO> GetByPublicIdAsync(long publicId);
        Task<List<CountryDTO>> GetCountriesWithUsersAsync();
        Task<UserProfileDTO> UpdateAsync(UserProfileDTO user);
        Task<List<UserProfileDTO>> SearchAsync(string name);
        Task<List<UserProfileDTO>> SearchAsync(string search=null, int? ageFrom = null, int? ageTo = null, long? cityId = null, long? countryId = null,
            string activityConcurence = null, string aboutConcurence = null, int? sex = null, short? sort = 0,
            int? lastIndex = 0);
    }
}
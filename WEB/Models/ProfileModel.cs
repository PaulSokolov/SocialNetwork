using BusinessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class ProfileModel
    {
        //public string Id { get; set; }
        public long PublicId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool BirthDateIsHidden { get; set; }
        public string About { get; set; }
        public bool AboutIsHidden { get; set; }
        public string Activity { get; set; }
        public bool ActivityIsHidden { get; set; }
        public CityDTO City { get; set; }
        public string Email { get; set; }
        public bool EmailIsHidden { get; set; }
        public SexDTO? Sex { get; set; }
        public string Role { get; set; }
        public bool IsEditHidden { get; set; }
        public bool IsFriend { get; set; }
        public bool IsFollower { get; set; }
        public bool IsFollowed { get; set; }
        public ICollection<LanguageDTO> Languages { get; set; }
        public ICollection<UserProfileDTO> Friends { get; set; }
    }
}
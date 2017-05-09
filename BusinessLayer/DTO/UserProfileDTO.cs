using System;
using System.Collections.Generic;

namespace BusinessLayer.DTO
{
    public class UserProfileDTO
    {

        public string Id { get; set; }
        public long PublicId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public DateTime? LastVisitDateTime { get; set; }
        public string Avatar { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool BirthDateIsHidden { get; set; }
        public string About { get; set; }
        public bool AboutIsHidden { get; set; }
        public string Activity { get; set; }
        public bool ActivityIsHidden { get; set; }
        public long? CityId { get; set; }
        public string Email { get; set; }
        public bool EmailIsHidden { get; set; }
        public CityDTO City { get; set; }
        public SexDTO? Sex { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public ICollection<FriendDTO> Friends { get; set; }
        public ICollection<FriendDTO> RequestedFriends { get; set; }
        public ICollection<UserMessageDTO> SentMessages { get; set; }
        public ICollection<UserMessageDTO> ReceivedMessages { get; set; }
        public ICollection<LanguageDTO> Languages { get; set; }

        public UserProfileDTO()
        {
            RequestedFriends = new List<FriendDTO>();
            Friends = new List<FriendDTO>();
            SentMessages = new List<UserMessageDTO>();
            ReceivedMessages = new List<UserMessageDTO>();
            Languages = new List<LanguageDTO>();
        }

        public override bool Equals(object obj)
        {
            var temp = obj as UserProfileDTO;
            return temp != null && (About == temp.About
                                    && AboutIsHidden == temp.AboutIsHidden
                                    && ActivatedDate == temp.ActivatedDate
                                    && Activity == temp.Activity
                                    && ActivityIsHidden == temp.ActivityIsHidden
                                    && Address == temp.Address
                                    && Avatar == temp.Avatar
                                    && BirthDate == temp.BirthDate
                                    && BirthDateIsHidden == temp.BirthDateIsHidden
                                    && City == temp.City
                                    && CityId == temp.CityId
                                    && Email == temp.Email
                                    && EmailIsHidden == temp.EmailIsHidden
                                    && Id == temp.Id
                                    && LastName == temp.LastName
                                    && LastVisitDateTime == temp.LastVisitDateTime
                                    && Name == temp.Name
                                    && Password == temp.Password
                                    && Role == temp.Role
                                    && Sex == temp.Sex);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(UserProfileDTO obj1, UserProfileDTO obj2)
        {
            return !ReferenceEquals(obj1, null) ? obj1.Equals(obj2) : ReferenceEquals(obj2, null);
        }
        public static bool operator !=(UserProfileDTO obj1, UserProfileDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

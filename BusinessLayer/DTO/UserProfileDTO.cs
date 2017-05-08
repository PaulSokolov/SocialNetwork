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
            if (obj is UserProfileDTO)
            {
                var temp = obj as UserProfileDTO;
                return this.About == temp.About
                    && this.AboutIsHidden == temp.AboutIsHidden
                    && this.ActivatedDate == temp.ActivatedDate
                    && this.Activity == temp.Activity
                    && this.ActivityIsHidden == temp.ActivityIsHidden
                    && this.Address == temp.Address
                    && this.Avatar == temp.Avatar
                    && this.BirthDate == temp.BirthDate
                    && this.BirthDateIsHidden == temp.BirthDateIsHidden
                    && this.City == temp.City
                    && this.CityId == temp.CityId
                    && this.Email == temp.Email
                    && this.EmailIsHidden == temp.EmailIsHidden
                    && this.Id == temp.Id
                    && this.LastName == temp.LastName
                    && this.LastVisitDateTime == temp.LastVisitDateTime
                    && this.Name == temp.Name
                    && this.Password == temp.Password
                    && this.Role == temp.Role
                    && this.Sex == temp.Sex;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(UserProfileDTO obj1, UserProfileDTO obj2)
        {
            if (object.ReferenceEquals(obj1, null))
            {
                if (object.ReferenceEquals(obj2, null))
                    return true;
                return false;
            }
            return obj1.Equals(obj2);

        }
        public static bool operator !=(UserProfileDTO obj1, UserProfileDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

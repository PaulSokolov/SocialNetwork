using System;

namespace BusinessLayer.DTO
{
    public class FriendDTO
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public string RequestUserId { get; set; }
        public bool Confirmed { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool Deleted { get; set; }

        public UserProfileDTO User { get; set; }
        public UserProfileDTO Friended { get; set; }        
        public UserProfileDTO RequestUser { get; set; }

        public override bool Equals(object obj)
        {
            var temp = obj as FriendDTO;
            if(temp != null)
            {
                return ConfirmDate == temp.ConfirmDate
                    && Confirmed == temp.Confirmed
                    && Deleted == temp.Deleted
                    && DeleteDate == temp.DeleteDate
                    && FriendId == temp.FriendId
                    && Friended==temp.Friended
                    && Id == temp.Id
                    && RequestDate == temp.RequestDate
                    && RequestUser==temp.RequestUser
                    && RequestUserId == temp.RequestUserId
                    && User==temp.User
                    && UserId == temp.UserId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(FriendDTO obj1, FriendDTO obj2)
        {
            return !ReferenceEquals(obj1, null) ? obj1.Equals(obj2) : ReferenceEquals(obj2, null);
        }

        public static bool operator !=(FriendDTO obj1, FriendDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

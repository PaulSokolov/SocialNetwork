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
            if(obj is FriendDTO)
            {
                var temp = obj as FriendDTO;
                return this.ConfirmDate == temp.ConfirmDate
                    && this.Confirmed == temp.Confirmed
                    && this.Deleted == temp.Deleted
                    && this.DeleteDate == temp.DeleteDate
                    && this.FriendId == temp.FriendId
                    && this.Friended==temp.Friended
                    && this.Id == temp.Id
                    && this.RequestDate == temp.RequestDate
                    && this.RequestUser==temp.RequestUser
                    && this.RequestUserId == temp.RequestUserId
                    && this.User==temp.User
                    && this.UserId == temp.UserId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(FriendDTO obj1, FriendDTO obj2)
        {
            if (object.ReferenceEquals(obj1, null))
            {
                if (object.ReferenceEquals(obj2, null))
                    return true;
                return false;
            }
            return obj1.Equals(obj2);

        }

        public static bool operator !=(FriendDTO obj1, FriendDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

using System;

namespace BusinessLayer.DTO
{
    public class UserMessageDTO
    {
        public long Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public long MessageId { get; set; }
        public DateTime PostedDate { get; set; }
        public bool IsRead { get; set; }
        public virtual string Body { get; set; }

        public UserProfileDTO FromUser { get; set; }
        public UserProfileDTO ToUser { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public static bool operator==(UserMessageDTO obj1, UserMessageDTO obj2)
        {
            if (object.ReferenceEquals(obj1, null))
            {
                if (object.ReferenceEquals(obj2, null))
                    return true;
                return false;
            }
            return obj1.Equals(obj2);

        }

        public static bool operator!=(UserMessageDTO obj1, UserMessageDTO obj2)
        {
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj)
        {
            if (obj is UserMessageDTO)
            {
                var temp = obj as UserMessageDTO;
                return this.Body == temp.Body
                    && this.FromUser==temp.FromUser
                    && this.Id == temp.Id
                    && this.IsRead == temp.IsRead
                    && this.MessageId == temp.MessageId
                    && this.PostedDate == temp.PostedDate
                    && this.ToUser==temp.ToUser
                    && this.ToUserId == temp.ToUserId
                    && this.FromUserId == temp.FromUserId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

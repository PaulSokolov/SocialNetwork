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
        public string Body { get; set; }

        public UserProfileDTO FromUser { get; set; }
        public UserProfileDTO ToUser { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public static bool operator==(UserMessageDTO obj1, UserMessageDTO obj2)
        {
            return !ReferenceEquals(obj1, null) ? obj1.Equals(obj2) : ReferenceEquals(obj2, null);
        }

        public static bool operator!=(UserMessageDTO obj1, UserMessageDTO obj2)
        {
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj)
        {
            var temp = obj as UserMessageDTO;
            if (temp != null)
            {
                return Body == temp.Body
                    && FromUser==temp.FromUser
                    && Id == temp.Id
                    && IsRead == temp.IsRead
                    && MessageId == temp.MessageId
                    && PostedDate == temp.PostedDate
                    && ToUser==temp.ToUser
                    && ToUserId == temp.ToUserId
                    && FromUserId == temp.FromUserId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

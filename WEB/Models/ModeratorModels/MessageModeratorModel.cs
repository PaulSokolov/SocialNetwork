
using System;

namespace SocialNetwork.Models.ModeratorModels
{
    public class MessageModeratorModel
    {
        public long Id { get; set; }
        public string UserAvatar { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public long UserPublicId { get; set; }
        public string RecipientAvatar { get; set; }
        public string RecipientName { get; set; }
        public string RecipientSurname { get; set; }
        public long RecipientPublicId { get; set; }
        public string Body { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
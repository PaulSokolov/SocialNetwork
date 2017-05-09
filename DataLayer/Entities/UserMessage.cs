using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
    public class UserMessage : Entity
    {
        public long Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public DateTime PostedDate { get; set; }
        public bool IsRead { get; set; }
        public string Body { get; set; }
        [ForeignKey("FromUserId")]
        public virtual UserProfile FromUser { get; set; }
        [ForeignKey("ToUserId")]
        public virtual UserProfile ToUser { get; set; }
    }
}

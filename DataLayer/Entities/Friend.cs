using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
    public class Friend:Entity
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }
        [Key, Column(Order = 1)]
        public string FriendId { get; set; }
        public string RequestUserId { get; set; }
        public bool Confirmed { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool Deleted { get; set; }
        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
        [ForeignKey("FriendId")]
        public virtual UserProfile Friended { get; set; }
        [ForeignKey("RequestUserId")]
        public virtual UserProfile RequestUser { get; set; }
    }
}

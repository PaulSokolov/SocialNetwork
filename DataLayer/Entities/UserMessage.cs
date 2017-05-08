using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

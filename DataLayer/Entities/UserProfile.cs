using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class UserProfile : Entity
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        [Required, Index(IsUnique = true), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PublicId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
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
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        public virtual ICollection<Language> Languages { get; set; }
        public virtual Sex? Sex { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }        
        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<Friend> RequestedFriends { get; set; }
        public virtual ICollection<UserMessage> SentMessages { get; set; }
        public virtual ICollection<UserMessage> ReceivedMessages { get; set; }

        public UserProfile()
        {
            RequestedFriends = new List<Friend>();
            Friends = new List<Friend>();
            SentMessages = new List<UserMessage>();
            ReceivedMessages = new List<UserMessage>();
            Languages = new List<Language>();
        }

    }
    public enum Sex
    {
        male,
        female
    }
}

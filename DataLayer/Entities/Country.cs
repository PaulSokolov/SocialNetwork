using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities
{
    public class Country:Entity
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Language> Languages { get; set; }
        public virtual ICollection<City> Cities { get; set; }
    }
}

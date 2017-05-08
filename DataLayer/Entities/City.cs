using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
    public class City : Entity
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public long CountryId { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
    }
}

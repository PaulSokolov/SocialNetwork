using System.Collections.Generic;
using System.Linq;
namespace BusinessLayer.DTO
{
    public class LanguageDTO
    {
        public long Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }

        public ICollection<CountryDTO> Countries { get; set; }
        public ICollection<UserProfileDTO> Users { get; set; }

        public LanguageDTO()
        {
            Countries = new List<CountryDTO>();
            Users = new List<UserProfileDTO>();
        }

        public override bool Equals(object obj)
        {
            if (obj is LanguageDTO)
            {
                var temp = obj as LanguageDTO;
                return this.Code == temp.Code
                    && this.Countries.SequenceEqual(temp.Countries)
                    && this.Name == temp.Name
                    && this.Id == temp.Id
                    && this.Users.SequenceEqual(temp.Users);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(LanguageDTO obj1, LanguageDTO obj2)
        {
            if (object.ReferenceEquals(obj1, null))
            {
                if (object.ReferenceEquals(obj2, null))
                    return true;
                return false;
            }
            return obj1.Equals(obj2);

        }

        public static bool operator !=(LanguageDTO obj1, LanguageDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

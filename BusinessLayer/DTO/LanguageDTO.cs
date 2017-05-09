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
            var temp = obj as LanguageDTO;
            return temp != null && (Code == temp.Code
                                    && Countries.SequenceEqual(temp.Countries)
                                    && Name == temp.Name
                                    && Id == temp.Id
                                    && Users.SequenceEqual(temp.Users));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(LanguageDTO obj1, LanguageDTO obj2)
        {
            return !ReferenceEquals(obj1, null) ? obj1.Equals(obj2) : ReferenceEquals(obj2, null);
        }

        public static bool operator !=(LanguageDTO obj1, LanguageDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

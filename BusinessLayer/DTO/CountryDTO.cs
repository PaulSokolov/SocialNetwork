using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.DTO
{
    public class CountryDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<LanguageDTO> Languages { get; set; }
        public ICollection<CityDTO> Cities { get; set; }

        public CountryDTO()
        {
            Languages = new List<LanguageDTO>();
            Cities = new List<CityDTO>();
        }

        public override bool Equals(object obj)
        {
            var temp = obj as CountryDTO;
            return temp != null && Id == temp.Id && Languages.SequenceEqual(temp.Languages) && Name == temp.Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CountryDTO obj1, CountryDTO obj2)
        {
            return !ReferenceEquals(obj1, null) ? obj1.Equals(obj2) : ReferenceEquals(obj2, null);
        }

        public static bool operator !=(CountryDTO obj1, CountryDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

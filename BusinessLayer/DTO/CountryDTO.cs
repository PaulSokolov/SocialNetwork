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
            if (obj is CountryDTO)
            {
                var temp = obj as CountryDTO;
                return this.Id == temp.Id
                    && this.Languages.SequenceEqual(temp.Languages)
                    && this.Name == temp.Name;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CountryDTO obj1, CountryDTO obj2)
        {
            if (object.ReferenceEquals(obj1, null))
            {
                if (object.ReferenceEquals(obj2, null))
                    return true;
                return false;
            }

            return obj1.Equals(obj2);

        }

        public static bool operator !=(CountryDTO obj1, CountryDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

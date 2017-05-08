namespace BusinessLayer.DTO
{
    public class CityDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CountryId { get; set; }

        public CountryDTO Country { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is CityDTO)
            {
                var temp = obj as CityDTO;
                return this.Id == temp.Id
                    && this.Country==temp.Country
                    && this.CountryId == temp.CountryId
                    && this.Name == temp.Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CityDTO obj1, CityDTO obj2)
        {
            if (object.ReferenceEquals(obj1, null))
            {
                if (object.ReferenceEquals(obj2, null))
                    return true;
                return false;
            }
            return obj1.Equals(obj2);

        }

        public static bool operator !=(CityDTO obj1, CityDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

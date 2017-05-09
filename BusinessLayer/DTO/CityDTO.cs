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
            var city = obj as CityDTO;
            if (city == null) return false;
            var temp = city;
            return Id == temp.Id
                   && Country==temp.Country
                   && CountryId == temp.CountryId
                   && Name == temp.Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CityDTO obj1, CityDTO obj2)
        {
            return !ReferenceEquals(obj1, null) ? obj1.Equals(obj2) : ReferenceEquals(obj2, null);
        }

        public static bool operator !=(CityDTO obj1, CityDTO obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

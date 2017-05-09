using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.DTO;

namespace SocialNetwork.Models
{
    public class SettingsViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool BirthDateIsHidden { get; set; }
        [DataType(DataType.MultilineText)]
        public string About { get; set; }
        public bool AboutIsHidden { get; set; }
        [DataType(DataType.MultilineText)]
        public string Activity { get; set; }
        public bool ActivityIsHidden { get; set; }
        public long? CityId { get; set; }
        public IEnumerable<CityViewModel> Cities{get;set;}
        public long? CountryId { get; set; }
        public IEnumerable<CountryViewModel> Countries { get; set; }
        public string Email { get; set; }
        public bool EmailIsHidden { get; set; }
        public SexDTO? Sex { get; set; }
    }
    public class CityViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class CountryViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
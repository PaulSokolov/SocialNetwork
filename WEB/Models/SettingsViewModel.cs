using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.DTO;

namespace SocialNetwork.Models
{
    public class SettingsViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Address { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public DateTime? BirthDate { get; set; }
        [Required]
        public bool BirthDateIsHidden { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }
        [Required]
        public bool AboutIsHidden { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Activity { get; set; }
        [Required]
        public bool ActivityIsHidden { get; set; }
        [Required]
        public long? CityId { get; set; }
        public IEnumerable<CityViewModel> Cities{get;set;}
        [Required]
        public long? CountryId { get; set; }
        public IEnumerable<CountryViewModel> Countries { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool EmailIsHidden { get; set; }
        [Required]
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
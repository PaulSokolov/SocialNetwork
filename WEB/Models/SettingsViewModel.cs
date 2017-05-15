using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages;
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
        [Required,DisplayName("Hide birthdate")]
        public bool BirthDateIsHidden { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }
        [Required, DisplayName("Hide about")]
        public bool AboutIsHidden { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Activity { get; set; }
        [Required, DisplayName("Hide activities")]
        public bool ActivityIsHidden { get; set; }
        [Required, DisplayName("City")]
        public long? CityId { get; set; }
        public IEnumerable<CityViewModel> Cities{get;set;}
        [Required, DisplayName("Country")]
        public long? CountryId { get; set; }
        public IEnumerable<CountryViewModel> Countries { get; set; }
        [Required]
        public string Email { get; set; }
        [Required, DisplayName("Hide email")]
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
﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.DTO;

namespace SocialNetwork.Models
{
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password"),DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [DisplayName("Choose Country")]
        [Required]
        public long CountryId { get; set; }
        [DisplayName("Choose City")]
        [Required]
        public long CityId { get; set; } 
        [Required]
        public  string Address { get; set; }
        [DataType(DataType.Date)]
        [Required, DisplayName("Birthdate")]
        public DateTime? BirthDate { get; set; }
        [Required]
        public SexDTO Sex { get; set; }

    }
}
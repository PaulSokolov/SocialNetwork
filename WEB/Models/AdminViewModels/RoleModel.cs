using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models.AdminViewModels
{
    public class RoleModel
    {
        [Required]
        public string Name { get; set; }
    }
}
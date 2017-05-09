using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.AdminViewModels
{
    public class RoleModel
    {
        [Required]
        public string Name { get; set; }
    }
}
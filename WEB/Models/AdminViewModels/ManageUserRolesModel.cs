using System.Collections.Generic;

namespace SocialNetwork.Models.AdminViewModels
{
    public class ManageUserRolesModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public long PublicId { get; set; }
        public string Avatar { get; set; }
        public List<string> Roles { get; set; }
        public List<string> AvailableRoles { get; set; }
    }
}
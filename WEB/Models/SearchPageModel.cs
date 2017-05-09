using System.Collections.Generic;
using BusinessLayer.DTO;

namespace SocialNetwork.Models
{
    public class SearchPageModel
    {

        public List<CountryDTO> Countries { get; set; }
        public List<UserSearchModel> Users { get; set; }
    }
}
using BusinessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class SearchPageModel
    {

        public List<CountryDTO> Countries { get; set; }
        public List<UserSearchModel> Users { get; set; }
    }
}
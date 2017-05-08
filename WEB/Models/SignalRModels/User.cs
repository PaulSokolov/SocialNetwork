using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models.SignalRModels
{
    public class User
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string PublicId { get; set; }
    }
}
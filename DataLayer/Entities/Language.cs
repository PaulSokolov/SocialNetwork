﻿using System.Collections.Generic;

namespace DataLayer.Entities
{
    public class Language:Entity
    {
        public long Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Country> Countries { get; set; }
        public virtual ICollection<UserProfile> Users { get; set; }

        public Language()
        {
            Countries = new List<Country>();
            Users = new List<UserProfile>();
        }
    }
}

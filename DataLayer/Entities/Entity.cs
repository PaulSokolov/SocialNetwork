using System;

namespace DataLayer.Entities
{
    public abstract class Entity
    {
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}

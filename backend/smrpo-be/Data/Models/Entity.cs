using System;

namespace smrpo_be.Data.Models
{
    public class Entity
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}

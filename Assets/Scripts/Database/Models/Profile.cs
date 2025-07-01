using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace Models
{
    [Table("profiles")]
    public class Profile : BaseModel
    {
        [PrimaryKey("user_id", true)]
        public Guid UserId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("surname")]
        public string Surname { get; set; }

        [Column("role")]
        public UserRole Role { get; set; }
    }
}
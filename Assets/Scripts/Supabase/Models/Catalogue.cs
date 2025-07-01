using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace Models
{
    [Table("catalogues")]
    public class Catalogue : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("topic_name")]
        public string TopicName { get; set; }

        [Column("is_private")]
        public bool IsPrivate { get; set; }

        [Column("created_by")]
        public Guid CreatedBy { get; set; }
    }
}

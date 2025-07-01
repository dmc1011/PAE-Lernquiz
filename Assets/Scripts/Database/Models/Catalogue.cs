using Postgrest.Attributes;
using Postgrest.Models;

namespace Models
{
    [Table("catalogues")]
    public class Catalogue : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("topic_id")]
        public int TopicId { get; set; }
    }
}

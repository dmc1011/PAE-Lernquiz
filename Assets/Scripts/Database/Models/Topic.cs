using Postgrest.Attributes;
using Postgrest.Models;

namespace Models
{
    [Table("topics")]
    public class Topic : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
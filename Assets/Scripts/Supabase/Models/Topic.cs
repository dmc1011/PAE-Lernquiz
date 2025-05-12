using Postgrest.Attributes;
using Postgrest.Models;

namespace Models
{
    [Table("topics")]
    public class Topic : BaseModel
    {
        [PrimaryKey("name", true)]
        public string Name { get; set; }
    }
}
using Postgrest.Attributes;
using Postgrest.Models;

namespace Models
{
    [Table("questions")]
    public class Question : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("catalogue_id")]
        public int CatalogueId { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }
    }
}
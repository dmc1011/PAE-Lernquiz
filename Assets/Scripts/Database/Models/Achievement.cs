using Postgrest.Attributes;
using Postgrest.Models;

namespace Models
{
    [Table("achievements")]
    public class Achievement : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("grade")]
        public string Grade { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("popup_text")]
        public string PopupText { get; set; }
    }
}

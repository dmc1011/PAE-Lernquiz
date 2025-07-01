using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace Models
{
    [Table("user_catalogue_history")]
    public class UserCatalogueHistory : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [PrimaryKey("user_id", false)]
        public Guid UserId { get; set; }

        [PrimaryKey("catalogue_id", false)]
        public int CatalogueId { get; set; }

        [Column("session_date")]
        public DateTime SessionDate { get; set; }

        [Column("time_spent")]
        public int TimeSpent { get; set; }

        [Column("is_complete")]
        public bool IsComplete { get; set; }

        [Column("is_error_free")]
        public bool IsErrorFree { get; set; }
    }
}

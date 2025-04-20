using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace Models
{
    [Table("user_catalogue_history")]
    public class UserCatalogueHistory : BaseModel
    {
        [PrimaryKey("id", true)]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("catalogue_id")]
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

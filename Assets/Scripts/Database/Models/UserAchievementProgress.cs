using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace Models
{
    [Table("user_achievement_progress")]
    public class UserAchievementProgress : BaseModel
    {
        [PrimaryKey("achievement_id", false)]
        public int AchievementId { get; set; }

        [PrimaryKey("user_id", false)]
        public Guid UserId { get; set; }

        [Column("is_achieved")]
        public bool IsAchieved { get; set; }

        [Column("achieved_at")]
        public DateTime? AchievedAt { get; set; }
    }
}
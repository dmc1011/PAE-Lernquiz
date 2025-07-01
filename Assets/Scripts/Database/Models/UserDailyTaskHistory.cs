using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace Models
{
    [Table("user_daily_task_history")]
    public class UserDailyTaskHistory : BaseModel
    {
        [PrimaryKey("user_id", false)]
        public Guid UserId { get; set; }

        [PrimaryKey("date", false)]
        public DateTime Date { get; set; }

        [Column("correct_answers_count")]
        public int CorrectAnswersCount { get; set; }

        [Column("task_completed")]
        public bool TaskCompleted { get; set; }
    }
}
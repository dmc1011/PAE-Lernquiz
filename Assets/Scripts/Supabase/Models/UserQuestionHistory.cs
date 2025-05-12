using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace Models
{
    [Table("user_question_history")]
    public class UserQuestionHistory : BaseModel
    {
        [PrimaryKey("id", true)]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("question_id")]
        public int QuestionId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("was_correct")]
        public bool WasCorrect { get; set; }

        [Column("session_id")]
        public int SessionId { get; set; }
    }
}
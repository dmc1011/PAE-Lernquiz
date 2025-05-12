using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace Models
{
    [Table("user_question_progress")]
    public class UserQuestionProgress : BaseModel
    {
        [PrimaryKey("question_id", false)]
        public int QuestionId { get; set; }

        [PrimaryKey("user_id", false)]
        public Guid UserId { get; set; }

        [Column("answer_count")]
        public int AnswerCount { get; set; }

        [Column("correct_answer_count")]
        public int CorrectAnswerCount { get; set; }

        [Column("in_practice")]
        public bool InPractice { get; set; }
    }
}
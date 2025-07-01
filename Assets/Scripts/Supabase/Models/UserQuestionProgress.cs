using Entities;
using Postgrest.Attributes;
using Postgrest.Models;
using Supabase.Gotrue;
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

        public UserQuestionProgress() { }

        public UserQuestionProgress(Guid userId, int questionId)
        {
            QuestionId = questionId;
            UserId = userId;
            AnswerCount = 0;
            CorrectAnswerCount = 0;
            InPractice = false;
        }
    }

}
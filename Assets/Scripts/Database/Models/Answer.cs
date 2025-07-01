using Postgrest.Attributes;
using Postgrest.Models;

namespace Models
{
    [Table("answers")]
    public class Answer : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("question_id")]
        public int QuestionId { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [Column("is_correct")]
        public bool IsCorrect { get; set; }
    }
}
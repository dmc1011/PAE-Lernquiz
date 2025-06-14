using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace Models
{
    [Table("user_catalogue_progress")]
    public class UserCatalogueProgress : BaseModel
    {
        [PrimaryKey("catalogue_id", false)]
        public int CatalogueId { get; set; }

        [PrimaryKey("user_id", false)]
        public Guid UserId { get; set; }

        [Column("current_question_id")]
        public int? CurrentQuestionId { get; set; }

        [Column("time_spent")]
        public int TimeSpent { get; set; }

        [Column("level")]
        public int Level { get; set; }

        [Column("error_free_runs")]
        public int ErrorFreeRuns { get; set; }

        [Column("random_quiz_count")]
        public int RandomQuizCount { get; set; }

        [Column("error_free_random_quiz_count")]
        public int ErrorFreeRandomQuizCount { get; set; }
        
        public UserCatalogueProgress() { }

        public UserCatalogueProgress(Guid userId, int catalogueId)
        {
            CatalogueId = catalogueId;
            UserId = userId;
            CurrentQuestionId = 0;
            TimeSpent = 0;
            Level = 0;
            ErrorFreeRuns = 0;
            RandomQuizCount = 0;
            ErrorFreeRandomQuizCount = 0;
        }
    }

}


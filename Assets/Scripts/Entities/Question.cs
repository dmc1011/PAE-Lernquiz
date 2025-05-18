using System.Collections.Generic;

namespace Entities 
{
    [System.Serializable]
    public class Question
    {
        public int id;
        public string text;
        public int catalogueId;
        public string name;
        public int correctAnsweredCount;
        public bool enabledForPractice;
        public int totalAnsweredCount;
        // TODO: questionImage
        public List<Answer> answers;
        public List<AnswerHistory> answerHistory;

        public Question(int id, string text, string name, int correctAnsweredCount, int totalAnsweredCount, int catalogueId, bool enabledForPractice, List<Answer> answers, List<AnswerHistory> answerHistory)
        {
            this.id = id;
            this.text = text;
            this.name = name;
            this.correctAnsweredCount = correctAnsweredCount;
            this.totalAnsweredCount = totalAnsweredCount;
            this.catalogueId = catalogueId;
            this.answers = answers;
            this.answerHistory = answerHistory;
            this.enabledForPractice = enabledForPractice;
        }
    }
}

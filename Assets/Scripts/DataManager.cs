using System.Collections.Generic;
using Entities;


public static class DataManager
{

    public static List<QuestionResult> QuestionResults = new();
    

    public struct QuestionRound
    {
        public int catalogueIndex;
        public Catalogue catalogue;
        public List<int> questions;         // chosen questions (incdices) for random quiz
        public int questionLimit;           // number of questions for random quiz
        public GameMode gameMode;    // saves the chosen game mode of current question round (e.g. "RandomQuiz")
    }


    public struct QuestionResult
    {
        public string questionText;
        public List<string> answerTexts;
        public string selectedAnswerText;
        // TODO: add questionImage
        // TODO: add answerImage
        public bool isCorrect;
        public int questionId;

        public QuestionResult(int questionIndex, int answerIndex, Catalogue catalogue)
        {
            isCorrect = answerIndex == 0;
            Question question = catalogue.questions[questionIndex];
            Answer answer = question.answers[answerIndex];
            List<string> allAnswerTexts = new() {
                question.answers[0].text,
                question.answers[1].text,
                question.answers[2].text,
                question.answers[3].text
            };
            questionText = question.text;
            answerTexts = allAnswerTexts;
            selectedAnswerText = answer.text;
            questionId = question.id;
        }
    }


    public struct DailyTask
    {
        public int catalogueIndex;
        public Catalogue catalogue;
        public List<int> questions; // chosen questions (incdices) for random quiz
        public int questionLimit;
        public List<QuestionResult> answers;
    }


    public static void AddAnswer(int questionIndex, int answerIndex, Catalogue catalogue)
    {
        QuestionResults.Add(new QuestionResult(questionIndex, answerIndex, catalogue));
    }

    public static void ClearResults()
    {
        QuestionResults.Clear();
    }

}

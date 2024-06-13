using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataManager
{

    public static List<QuestionResult> QuestionResults = new();
    
    public struct QuestionRound
    {
        public int catalogueIndex;
        public Catalogue catalogue;
        public List<int> questions; // chosen questions (incdices) for random quiz
        public int questionLimit; // Number of questions in a random quiz
    }

    public struct QuestionResult
    {
        public string questionText;
        public string answerText;
        // TODO: add questionImage
        // TODO: add answerImage
        public bool isCorrect;
    }

    public static void AddAnswer(int questionIndex, int answerIndex, Catalogue catalogue)
    {
        bool isCorrect = answerIndex == 0;
        Question question = catalogue.questions[questionIndex];
        Answer answer = question.answers[answerIndex];

        QuestionResults.Add(new QuestionResult
        {
            questionText = question.text,
            answerText = answer.text,
            // TODO: questionImage = question.image
            // TODO: answerImage = answer.image
            isCorrect = isCorrect
        });
    }

    public static void ClearResults()
    {
        QuestionResults.Clear();
    }

}

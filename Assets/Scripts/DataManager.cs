using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataManager
{

    public static List<QuestionResult> QuestionResults = new();
    
    public struct QuestionRound
    {
        public int CatalogueIndex; // Index in der Liste aller Kataloge
        public List<int> Questions; // Indices der Fragen im Katalog
        public int QuestionCounter; // Zählt hoch bis AnzahlFragenProFragerunde, danach endet die Fragerunde.
    }

    public struct QuestionResult
    {
        public string questionText;
        public string givenAnswer;
        public bool isCorrect;
    }

    public static void AddAnswer(string questionText, string givenAnswer, bool isCorrect)
    {
        QuestionResults.Add(new QuestionResult
        {
            questionText = questionText,
            givenAnswer = givenAnswer,
            isCorrect = isCorrect
        });
    }

    public static void ClearResults()
    {
        QuestionResults.Clear();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Answer
{
    public int id;
    public string text;
    public int questionId;
    // TODO: answerImage
    public bool isCorrect;

    public Answer (int id, string text, int questionId, bool isCorrect)
    {
        this.id = id;
        this.text = text;
        this.questionId = questionId;
        this.isCorrect = isCorrect;
    }
}

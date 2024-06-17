using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Answer
{
    public string id;
    public string text;
    public string questionId;
    // TODO: answerImage
    public bool isCorrect;

    public Answer (string id, string text, string questionId, bool isCorrect)
    {
        this.id = id;
        this.text = text;
        this.questionId = questionId;
        this.isCorrect = isCorrect;
    }
}

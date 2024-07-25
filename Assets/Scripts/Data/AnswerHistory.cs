using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnswerHistory
{
    public int id;
    public int questionId;
    public DateTime answerDate;
    public bool wasCorrect;

    public AnswerHistory (int id, int questionId, DateTime answerDate, bool wasCorrect)
    {
        this.id = id;
        this.questionId = questionId;
        this.answerDate = answerDate;
        this.wasCorrect = wasCorrect;
    }
}

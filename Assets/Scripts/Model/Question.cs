using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string id;
    public string questionInfo;
    public int correctAnswerIndex;
    public List<string> answers;
}

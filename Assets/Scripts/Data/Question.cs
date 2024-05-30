using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string id;
    public string text;
    // TODO: questionImage
    public List<Answer> answers;
}

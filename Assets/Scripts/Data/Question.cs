using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string id;
    public string text;
    public string catalogueId;
    // TODO: questionImage
    public List<Answer> answers;

    public Question(string id, string text, string catalogueId, List<Answer> answers)
    {
        this.id = id;
        this.text = text;
        this.catalogueId = catalogueId;
        this.answers = answers;
    }
}

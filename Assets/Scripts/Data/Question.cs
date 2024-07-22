
using System.Collections.Generic;


[System.Serializable]
public class Question
{
    public int id;
    public string text;
    public int catalogueId;
    public string name;
    // TODO: questionImage
    public List<Answer> answers;

    public Question(int id, string text, string name, int catalogueId, List<Answer> answers)
    {
        this.id = id;
        this.text = text;
        this.name = name;
        this.catalogueId = catalogueId;
        this.answers = answers;
    }
}

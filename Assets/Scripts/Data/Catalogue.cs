using System.Collections.Generic;

[System.Serializable]
public class Catalogue
{
    public int id;
    public string name;
    public int currentQuestionId;
    public List<Question> questions;

    public Catalogue (int id, string name, int currentQuestionId, List<Question> questions)
    {
        this.id = id;
        this.name = name;
        this.currentQuestionId = currentQuestionId;
        this.questions = questions;
    }
}

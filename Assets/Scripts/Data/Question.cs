using System.Collections.Generic;

[System.Serializable]
public class Question
{
    public int id;
    public string text;
    public int catalogueId;
    public string name;
    public bool correctAnswered;
    // TODO: questionImage
    public List<Answer> answers;
    public List<AnswerHistory> answerHistory;

    public Question(int id, string text, string name, bool correctAnswered, int catalogueId, List<Answer> answers, List<AnswerHistory> answerHistory)
    {
        this.id = id;
        this.text = text;
        this.name = name;
        this.correctAnswered = correctAnswered;
        this.catalogueId = catalogueId;
        this.answers = answers;
        this.answerHistory = answerHistory;
    }
}

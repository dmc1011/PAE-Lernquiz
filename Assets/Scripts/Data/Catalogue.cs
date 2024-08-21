using System.Collections.Generic;

[System.Serializable]
public class Catalogue
{
    public int id;
    public string name;
    public int currentQuestionId;
    public int totalTimeSpent;
    public int sessionCount;
    public int errorFreeSessionCount;
    public List<Question> questions;
    public List<CatalogueSessionHistory> sessionHistories;

    public Catalogue (int id, string name, int currentQuestionId, int totalTimeSpent, int sessionCount, int errorFreeSessionCount, List<Question> questions, List<CatalogueSessionHistory> sessionHistories)
    {
        this.id = id;
        this.name = name;
        this.currentQuestionId = currentQuestionId;
        this.totalTimeSpent = totalTimeSpent;
        this.sessionCount = sessionCount;
        this.errorFreeSessionCount = errorFreeSessionCount;
        this.questions = questions;
        this.sessionHistories = sessionHistories;
    }
}

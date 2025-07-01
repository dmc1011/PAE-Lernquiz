using System;

[System.Serializable]
public class DailyTaskHistory
{
    public int id;
    public DateTime taskDate;
    public int correctAnswersCount;
    public bool taskCompleted;

    public DailyTaskHistory(int id, DateTime taskDate, int correctAnswersCount, bool taskCompleted)
    {
        this.id = id;
        this.taskDate = taskDate;
        this.correctAnswersCount = correctAnswersCount;
        this.taskCompleted = taskCompleted;
    }
}

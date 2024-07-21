using UnityEngine;

public static class Global
{
    // player prefs keys for daily task
    public const string LastResetDateKey = "LastResetDate";
    public const string IsDailyTaskCompletedKey = "IsDailyTaskCompleted";

    // monitoring question round
    public static bool InsideQuestionRound = false;
    public static DataManager.QuestionRound CurrentQuestionRound;
    public const int RandomQuizSize = 5;

    // monitoring daily task
    public static DataManager.DailyTask CurrentDailyTask;
    public const int DailyTaskSize = 10;
    public static Catalogue tmpCatalogue = null;
}


using UnityEngine;

public static class Global
{
    // Es folgen globale Einstellungen f�r die App
    // Diese k�nnen nicht via Code ge�ndert werden.
    public static readonly int NumQuestionsPerRound = 5;

    // player prefs keys
    public const string LastResetDateKey = "LastResetDate";
    public const string IsDailyTaskCompletedKey = "IsDailyTaskCompleted";

    // monitoring question round
    public static bool InsideQuestionRound = false;
    public static DataManager.QuestionRound CurrentQuestionRound;

    // monitoring daily task
    public static bool isDailyTaskCompleted = PlayerPrefs.GetString(IsDailyTaskCompletedKey, "") == "true";
    public static DataManager.DailyTask CurrentDailyTask;
}


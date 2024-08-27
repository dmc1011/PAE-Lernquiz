
public static class Global
{
    // GameMode is used as context for catalogue selection: selecting a catalogue will trigger specific events and screen transition
    public enum GameMode
    {
        LinearQuiz,
        RandomQuiz,
        DailyTask,
        Statistics,
        PracticeBook
    }

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



using System.Collections.Generic;

public static class Global
{
    // GameMode is used as context for catalogue selection: selecting a catalogue will trigger specific events and screen transition
    public enum GameMode
    {
        LinearQuiz,
        RandomQuiz,
        DailyTask,
        Statistics,
        Editor
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


    // returns true if tmpCatalogue != null
    public static bool SetTmpCatalogue (string catalogueName)
    {
        if (catalogueName == null)
        {
            tmpCatalogue = new Catalogue(0, "Neuer Fragenkatalog", 0, 0, new List<Question>(), new List<CatalogueSessionHistory>());
            return true;
        }

        CatalogueTable catalogueTable = SQLiteSetup.Instance.catalogueTable;
        tmpCatalogue = catalogueTable.FindCatalogueByName(catalogueName);
        return tmpCatalogue != null;
    }
}

